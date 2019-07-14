using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;
using Model.Requests;

namespace ProductScraper.Scrapers
{
    public class JumboScraper : IProductScraper
    {
        readonly IBrowsingContext _browsingContext;
        readonly ApplicationContext _context;
        readonly ProductApplicationService _productService;
        readonly DateTime _scrapeDate;
        readonly StreamWriter _streamWriter;

        public JumboScraper(ApplicationContext context, IMapper mapper, StreamWriter streamWriter, DateTime scrapeDate)
        {
            _context = context;
            _streamWriter = streamWriter;
            _scrapeDate = scrapeDate;

            _productService = new ProductApplicationService(_context, mapper);

            IConfiguration config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task ScrapeAll()
        {
            var productUrlDictonary = new Dictionary<int, List<string>>();

            //Get main categorie url's
            var productCategories = _context.ProductCategories
                    .Include(_ => _.StoreCategories)
                    .Where(_ => _.StoreCategories.Any(sc => sc.StoreType == StoreType.Jumbo));

            //Get product url's from categories
            foreach (var productCategorie in productCategories)
            {
                var productCategorieUrls = productCategorie.StoreCategories.Where(_ => _.StoreType == StoreType.Jumbo).Select(_ => _.Url);
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

            //Scrape products that have not been found in a category
            var notFoundProducts = _context.Products
                .Include(p => p.ProductProductCategories)
                .Where(_ => _.StoreType == StoreType.Jumbo && _.LastScrapeDate != _scrapeDate);
            foreach (var notFoundProduct in notFoundProducts)
            {
                await HandleProduct(notFoundProduct.Url, notFoundProduct.ProductCategories.First());
            }

            //Remove outdated products
            _productService.RemoveOutdatedProducts(StoreType.Jumbo, _scrapeDate);
        }

        public async Task ScrapeProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductProductCategories)
                .First(_ => _.Id == id);
            await HandleProduct(product.Url, product.ProductCategories.First());
        }

        async Task<List<string>> GetProductUrls(string url)
        {
            var productUrls = new List<string>();
            var productCategoryUrls = new List<string>();

            try {
                var request = DocumentRequest.Get(Url.Create(url));
                request.Headers.Add("User-Agent", "chrome"); //Needed to authorize request

                IDocument categoryDocument;
                var reloadCount = 0;
                do
                {
                    categoryDocument = await _browsingContext.OpenAsync(request);

                    reloadCount++;
                    if (reloadCount > 4)
                    {
                        break;
                    }
                }
                while (categoryDocument.QuerySelector(".jumbo-search-results-header") == null);

                if (categoryDocument.QuerySelector(".jum-has-breadcrumb-sub button").TextContent.ToLower() == "kies een schap")
                {
                    var productCategoryUrlsFound = categoryDocument
                        .QuerySelectorAll(".jum-breadcrumb-sub .jum-breadcrumb-col a")
                        .SelectMany(_ => _.Attributes.Where(a => a.Name == "href").Select(href => href.Value));
                    var trimmedUrls = productCategoryUrlsFound.Select(_ => _.Contains(";") ? _.Substring(0, _.IndexOf(";", StringComparison.Ordinal)) : _).ToList();
                    productCategoryUrls.AddRange(trimmedUrls);
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
            else
            {
                var productsFound = 0;
                var pageCounter = 0;
                do
                {
                    var productsRequest = DocumentRequest.Get(Url.Create(url + "?PageNumber=" + pageCounter));
                    productsRequest.Headers.Add("User-Agent", "chrome"); //Needed to authorize request
                    var productListDocument = await _browsingContext.OpenAsync(productsRequest);

                    var productsUrlFound = productListDocument
                        .QuerySelectorAll("div.jum-item-product")
                        .Where(_ => !_.QuerySelectorAll("button.jum-add").Any(p => p.TextContent.ToLower() == "voeg set toe aan lijst")) //Remove product sets
                        .SelectMany(_ => _.QuerySelector("a").Attributes.Where(a => a.Name == "href").Select(href => href.Value));

                    var trimmedUrls = productsUrlFound.Select(_ => _.Contains(";") ? _.Substring(0, _.IndexOf(";", StringComparison.Ordinal)) : _).ToList();
                    productUrls.AddRange(trimmedUrls);

                    productsFound = productsUrlFound.Count();

                    pageCounter++;
                }
                while (productsFound != 0);

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
                var request = DocumentRequest.Get(Url.Create(url));
                request.Headers.Add("User-Agent", "chrome"); //Needed to authorize request
                var productDocument = await _browsingContext.OpenAsync(request);

                var code = "";
                var codeMatch = Regex.Match(url, @"(?:https?:\/\/www\.jumbo.com\/[^\/.]*\/)(\w*)");
                if (codeMatch.Success)
                {
                    code = codeMatch.Groups[1].Value;
                }

                if (code == "")
                {
                    throw new ArgumentException("Product code is empty");
                }

                //Scrape product page
                var name = productDocument.QuerySelector(".jum-column-main h1").TextContent;
                name = Regex.Replace(name, @"[\u00AD]", ""); //Remove soft hypens from name
                var ammount = productDocument.QuerySelector(".jum-column-main .jum-pack-size").TextContent;
                var ingredients = GetIngredients(productDocument);
                var allergyInfo = GetAllergyInfo(productDocument);
                var isStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(productDocument);

                var productStoreRequest = new ProductStoreRequest
                {
                    StoreType = StoreType.Jumbo,
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

                _productService.CreateOrUpdate(productStoreRequest);

                var logLine = $"Handled product: {code} {name}";
                Console.WriteLine(logLine);
                _streamWriter.WriteLine(logLine);
            }
            catch (Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping product: {url}");
                _streamWriter.WriteLine(ex);
            }
        }

        private string GetIngredients(IDocument productDocument)
        {
            var replaceRegex = new List<string> {
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
                var elements = productDocument.QuerySelectorAll(".jum-ingredients-info li");
                ingredients = string.Join(", ", elements.Select(_ => _.TextContent.Trim().ToLower()));
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
            var allergyInfo = "";
            try
            {
                var elements = productDocument.QuerySelectorAll(".jum-product-allergy-info li");
                allergyInfo = string.Join(", ", elements.Where(_ => _.TextContent.ToLower().StartsWith("bevat", StringComparison.Ordinal)).Select(_ => _.TextContent.Replace("bevat", "").Trim().ToLower()));
            }
            catch
            {
                return allergyInfo;
            }

            return allergyInfo;
        }

        private bool GetIsStoreAdvertisedVegan(IDocument productDocument)
        {
            return productDocument.QuerySelectorAll(".jum-product-info-item").Any(_ => _.TextContent.ToLower().Contains("vegan"));
        }
    }
}
