using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;
using Model.Requests;
using Newtonsoft.Json.Linq;

namespace ProductScraper.Scrapers
{
    public class AlbertHeijnScraper : IProductScraper
    {
        readonly ApplicationContext _context;
        readonly ProductApplicationService _productService;
        readonly DateTime _scrapeDate;
        readonly StreamWriter _streamWriter;
        readonly IBrowsingContext _browsingContext;
        private static HttpClient _client = new HttpClient();

        public AlbertHeijnScraper(ApplicationContext context, IMapper mapper, StreamWriter streamWriter, DateTime scrapeDate)
        {
            _context = context;
            _streamWriter = streamWriter;
            _scrapeDate = scrapeDate;

            _productService = new ProductApplicationService(_context, mapper);
            _browsingContext = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
        }

        public async Task ScrapeAll()
        {
            var productUrlDictonary = new Dictionary<int, List<string>>();

            //Get main categorie url's
            var productCategories = _context.ProductCategories
                    .Include(_ => _.StoreCategories)
                    .Where(_ => _.StoreCategories.Any(sc => sc.StoreType == StoreType.AlbertHeijn));

            //Get product url's from categories
            foreach (var productCategorie in productCategories)
            {
                var productCategorieUrls = productCategorie.StoreCategories.Where(_ => _.StoreType == StoreType.AlbertHeijn).Select(_ => _.Url);
                foreach (var productCategorieUrl in productCategorieUrls)
                {
                    if (productUrlDictonary.ContainsKey(productCategorie.Id))
                    {
                        productUrlDictonary[productCategorie.Id].AddRange(await GetProductUrls(productCategorieUrl));
                    }
                    else
                    {
                        productUrlDictonary.Add(productCategorie.Id, await GetProductUrls(productCategorieUrl));
                    }
                }
            }

            //Get product data
            foreach (var productsUrlsForCategory in productUrlDictonary)
            {
                var productCateogry = _context.ProductCategories.Find(productsUrlsForCategory.Key);
                var productsUrlsForCategoryDistinct = productsUrlsForCategory.Value.Distinct().ToList();

                foreach (var productUrl in productsUrlsForCategoryDistinct)
                {
                    await HandleProduct(productUrl, productCateogry);
                }
            }

            var notFoundProducts = _context.Products
                .Include(p => p.ProductProductCategories)
                .Where(_ => _.StoreType == StoreType.AlbertHeijn && _.LastScrapeDate != _scrapeDate);
            foreach (var notFoundProduct in notFoundProducts)
            {
                await HandleProduct(notFoundProduct.Url, notFoundProduct.ProductCategories.First());
            }

            //Remove outdated products
            _productService.RemoveOutdatedProducts(StoreType.AlbertHeijn, _scrapeDate);
        }

        public async Task ScrapeCategory(string scrapeCategoryName)
        {
            var productUrls = new List<string>();
            var productCategorie = _context.ProductCategories
                .Include(_ => _.StoreCategories)
                .Single(_ => _.Name.ToLower() == scrapeCategoryName.ToLower());

            //Get product url's from category
            var productCategorieUrls = productCategorie.StoreCategories.Where(_ => _.StoreType == StoreType.AlbertHeijn).Select(_ => _.Url);
            foreach (var productCategorieUrl in productCategorieUrls)
            {
                productUrls.AddRange(await GetProductUrls(productCategorieUrl));
            }

            //Get product data
            foreach (string productUrl in productUrls)
            {
                await HandleProduct(productUrl, productCategorie);
            }
        }

        async Task<List<string>> GetProductUrls(string url)
        {
            var productUrls = new List<string>();
            var productCategoryUrls = new List<string>();
            var productLanes = new List<JToken>();

            try
            {
                using (var result = await _client.GetAsync("https://www.ah.nl/service/rest" + url))
                {
                    using (var content = result.Content)
                    {
                        string dataString = await content.ReadAsStringAsync();
                        if (dataString != null)
                        {
                            var data = JObject.Parse(dataString);
                            productLanes = data["_embedded"]?["lanes"]?
                                .Where(_ => _["type"].Value<string>() == "ProductLane")
                                .ToList();

                            var legends = productLanes
                                .SelectMany(_ => _["_embedded"]?["items"])
                                .Where(_ => _["type"]?.Value<string>() == "Legend");
                            if (legends.Any())
                            {
                                productCategoryUrls.AddRange(legends.Select(_ => _["navItem"]?["link"]?["href"].Value<string>()));
                            }

                            var seeMoreItems = data["_embedded"]?["lanes"]?
                                .Where(_ => _["type"].Value<string>() == "SeeMoreLane")
                                .SelectMany(_ => _["_embedded"]?["items"]);
                            if (seeMoreItems.Any())
                            {
                                productCategoryUrls.AddRange(seeMoreItems.Select(_ => _["navItem"]?["link"]?["href"].Value<string>()));
                            }
                        }
                        else
                        {
                            throw new ArgumentException("No data loaded");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping category: {url}");
                _streamWriter.WriteLine(ex);
            }

            //If there are sub categories found open these sub categories
            if (productCategoryUrls.Any())
            {
                foreach (string productCategoryUrl in productCategoryUrls)
                {
                    productUrls.AddRange(await GetProductUrls(productCategoryUrl));
                }
            }
            //If there are no sub categories found get product url's
            else
            {
                var products = productLanes
                    .SelectMany(_ => _["_embedded"]?["items"])
                    .Where(_ => _["type"]?.Value<string>() == "Product");
                productUrls.AddRange(products.Select(_ => "https://www.ah.nl" + _["navItem"]?["link"]?["href"].Value<string>()));

                var logLine = $"Scraped category: {url}. Found {productUrls.Count} products.";
                Console.WriteLine(logLine);
                _streamWriter.WriteLine(logLine);
            }

            return productUrls;
        }

        async Task HandleProduct(string url, ProductCategory productCategory)
        {
            try
            {
                var productDocument = await _browsingContext.OpenAsync(url);

                var code = "";
                var codeMatch = Regex.Match(url, @"(?:https?:\/\/www\.ah\.nl\/producten\/product\/)(\w*)");
                if (codeMatch.Success)
                {
                    code = codeMatch.Groups[1].Value;
                }

                if (code == "")
                {
                    throw new ArgumentException("Product code is empty");
                }

                //Scrape product page
                var name = productDocument.QuerySelector("div.product-hero h1 span").TextContent;
                name = Regex.Replace(name, @"[\u00AD]", ""); //Remove soft hypens from name
                var ingredients = GetIngredients(productDocument);
                var allergyInfo = GetAllergyInfo(productDocument);
                var isStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(productDocument);

                var request = new ProductStoreRequest
                {
                    StoreType = StoreType.AlbertHeijn,
                    Name = name,
                    Code = code,
                    Url = url,
                    Ingredients = ingredients,
                    AllergyInfo = allergyInfo,
                    IsStoreAdvertisedVegan = isStoreAdvertisedVegan,
                    LastScrapeDate = _scrapeDate,
                    ProductCategory = productCategory
                };

                _productService.CreateOrUpdate(request);

                var logLine = $"Handled product: {code} {name}";
                Console.WriteLine(logLine); 
                _streamWriter.WriteLine(logLine);
            }
            catch(Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping product: {url}");
                _streamWriter.WriteLine(ex);
            }
        }

        private string GetIngredients(IDocument productDocument) {
            var replaceRegex = new List<string> {
                @"ingrediënten:",
                @"ingredienten:",
                @"ingrediënts:",
                @"ingredients:",
                @"dit product.*",
                @"kan sporen.*",
                @"allergiewijzer.*",
                @"allergie info.*",
                @"allergie informatie.*",
                @"allergie-informatie.*",
                @"kan.*bevatten.*",
                @"may contain.*"
            };

            var ingredients = "";
            try
            {
                ingredients = productDocument
                    .QuerySelectorAll("div.product-info-ingredients h2")
                    .First(_ => _.TextContent == "ingrediënten")
                    .NextSibling.TextContent.ToLower();
            }
            catch
            {
                return ingredients;
            }

            foreach (var regex in replaceRegex)
            {
                ingredients = Regex.Replace(ingredients, regex, "");
            }

            return ingredients.Trim();
        }

        private string GetAllergyInfo(IDocument productDocument)
        {
            var replaceRegex = new List<string> {
                @"bevat:",
                @"kan bevatten.*"
            };

            var allergyInfo = "";
            try
            {
                allergyInfo = productDocument
                    .QuerySelectorAll("div.product-info-ingredients h4")
                    .First(_ => _.TextContent == "allergie-informatie")
                    .NextSibling.TextContent.ToLower();
            }
            catch
            {
                return allergyInfo;
            }

            foreach (var regex in replaceRegex)
            {
                allergyInfo = Regex.Replace(allergyInfo, regex, "");
            }

            return allergyInfo.Replace(".", "").Trim();
        }

        private bool GetIsStoreAdvertisedVegan(IDocument productDocument)
        {
            var isVegan = productDocument.QuerySelector("li.productcard-info__feature svg--vegan") != null
                || productDocument.QuerySelectorAll("li.productcard-info__feature p").Any(_ => _.TextContent.ToLower().Contains("vegan"));

            if (!isVegan)
            {
                var summaryNodes = productDocument.QuerySelector("div.product-hero div.product-summary");
                isVegan = summaryNodes != null && summaryNodes.TextContent.ToLower().Contains("vegan");
            }

            return isVegan;
        }
    }
}
