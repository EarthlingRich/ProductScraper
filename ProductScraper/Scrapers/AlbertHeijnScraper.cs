using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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

namespace ProductScraper.Scrapers
{
    public class AlbertHeijnScraper : IProductScraper
    {
        readonly string BaseUrl = "https://www.ah.nl";

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
                        productUrlDictonary[productCategorie.Id].AddRange(await GetProductUrls(productCategorieUrl, productCategorie));
                    }
                    else
                    {
                        productUrlDictonary.Add(productCategorie.Id, await GetProductUrls(productCategorieUrl, productCategorie));
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

            //Scrape products that have not been found in a category
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

        public async Task ScrapeProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductProductCategories)
                .First(_ => _.Id == id);
            await HandleProduct(product.Url, product.ProductCategories.First());
        }

        async Task<List<string>> GetProductUrls(string slug, ProductCategory productCategorie)
        {
            var logLineStart = $"Scraping category: {productCategorie.Name}";
            Console.WriteLine(logLineStart);
            _streamWriter.WriteLine(logLineStart);

            var productUrls = new List<string>();
            var taxonomyIds = new List<string>();
            var url = $"{BaseUrl}/zoeken/api/products/search?taxonomySlug={slug}&size=10000";

            try
            {
                using (var httpResponse = await _client.GetAsync(url))
                using (var stream = await httpResponse.Content.ReadAsStreamAsync())
                using (var data = JsonDocument.Parse(stream))
                {
                    foreach (var taxonomy in data.RootElement.GetProperty("aggregation").GetProperty("taxonomies").EnumerateArray())
                    {
                        taxonomyIds.Add(taxonomy.GetProperty("id").ToString());
                    }
                }
                if (taxonomyIds.Any())
                {
                    foreach (string taxonomyId in taxonomyIds)
                    {
                        url = $"{BaseUrl}/zoeken/api/products/search?taxonomySlug={slug}&taxonomy={taxonomyId}&size=10000";
                        using (var httpResponse = await _client.GetAsync(url))
                        using (var stream = await httpResponse.Content.ReadAsStreamAsync())
                        using (var data = JsonDocument.Parse(stream))
                        {
                            foreach (var cards in data.RootElement.GetProperty("cards").EnumerateArray())
                            {
                                foreach (var product in cards.GetProperty("products").EnumerateArray())
                                {
                                    productUrls.Add(product.GetProperty("link").ToString());
                                }
                            }
                        }
                    }
                }
                var logLineEnd = $"Found {productUrls.Count} products.";
                Console.WriteLine(logLineEnd);
                _streamWriter.WriteLine(logLineEnd);
            }
            catch (Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping: {url}");
                _streamWriter.WriteLine(ex);
            }

            return productUrls;
        }

        async Task HandleProduct(string url, ProductCategory productCategory)
        {
            try
            {
                url = BaseUrl + url;
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
                var name = productDocument.QuerySelector("article h1 span").TextContent;
                name = Regex.Replace(name, @"[\u00AD]", ""); //Remove soft hypens from name
                var ammount = GetAmmount(productDocument);
                var ingredients = GetIngredients(productDocument);
                var allergyInfo = GetAllergyInfo(productDocument);
                var isStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(productDocument);

                var request = new ProductStoreRequest
                {
                    StoreType = StoreType.AlbertHeijn,
                    Name = name,
                    Ammount = ammount,
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
                @"may contain.*",
                @"waarvan*"
            };

            var ingredients = "";
            try
            {
                ingredients = productDocument
                    .QuerySelector(".product-info-ingredients h2")
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
                    .QuerySelector(".product-info-ingredients h4")
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

        private string GetAmmount(IDocument productDocument)
        {
            var ammount = productDocument
                .QuerySelector("[class^='product-card-header_unitInfo']")
                .ChildNodes.OfType<IText>()
                .Select(m => m.Text)
                .FirstOrDefault();

            if (!Regex.Match(ammount, @"\d+\s*x\s*[\d.,\s]+[gmkl][lg]*", RegexOptions.IgnoreCase).Success)
            {
                var ammountResults = productDocument.QuerySelectorAll(".product-info-content-block")
                    .FirstOrDefault(_ => _.QuerySelector("h4").TextContent.ToLower() == "inhoud en gewicht")
                    ?.QuerySelectorAll("p")
                    .Select(_ => _.TextContent);

                foreach (var ammountResult in ammountResults)
                {
                    if (Regex.Match(ammountResult, @"^\d+\s*x\s*[\d.,\s]+[gmkl][lg]*$", RegexOptions.IgnoreCase).Success)
                    {
                        ammount = ammountResult;
                        break;
                    }
                }
            }

            return ammount;
        }

        private bool GetIsStoreAdvertisedVegan(IDocument productDocument)
        {
            var isVegan = productDocument.QuerySelectorAll("li.productcard-info__feature p").Any(_ => _.TextContent.ToLower().Contains("vegan"));

            if (!isVegan)
            {
                var summaryNodes = productDocument.QuerySelector("article .product-summary");
                isVegan = summaryNodes != null && summaryNodes.TextContent.ToLower().Contains("vegan");
            }

            return isVegan;
        }
    }
}
