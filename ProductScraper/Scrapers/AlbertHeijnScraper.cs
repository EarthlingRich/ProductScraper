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
            var requestDictonary = new Dictionary<int, List<ProductStoreRequest>>();

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
                    if (requestDictonary.ContainsKey(productCategorie.Id))
                    {
                        requestDictonary[productCategorie.Id].AddRange(await GetBareProducts(productCategorieUrl, productCategorie));
                    }
                    else
                    {
                        requestDictonary.Add(productCategorie.Id, await GetBareProducts(productCategorieUrl, productCategorie));
                    }
                }
            }

            //Get product data
            foreach (var productStoreRequestsForCategory in requestDictonary)
            {
                var productCateogry = _context.ProductCategories.Find(productStoreRequestsForCategory.Key);
                var productStoreRequestsForCategoryDistinct = productStoreRequestsForCategory.Value.Distinct().ToList();

                foreach (var productStoreRequest in productStoreRequestsForCategoryDistinct)
                {
                    await HandleProduct(productStoreRequest, productCateogry);
                }
            }

            //Scrape products that have not been found in a category
            var notFoundProducts = _context.Products
                .Include(p => p.ProductProductCategories)
                .Where(_ => _.StoreType == StoreType.AlbertHeijn && _.LastScrapeDate != _scrapeDate);
            foreach (var notFoundProduct in notFoundProducts)
            {
                await HandleProduct(ProductToBareRequest(notFoundProduct), notFoundProduct.ProductCategories.First());
            }

            //Remove outdated products
            _productService.RemoveOutdatedProducts(StoreType.AlbertHeijn, _scrapeDate);
        }

        public async Task ScrapeProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductProductCategories)
                .First(_ => _.Id == id);
            await HandleProduct(ProductToBareRequest(product), product.ProductCategories.First());
        }

        async Task<List<ProductStoreRequest>> GetBareProducts(string slug, ProductCategory productCategorie)
        {
            var logLineStart = $"Scraping category: {productCategorie.Name}";
            Console.WriteLine(logLineStart);
            _streamWriter.WriteLine(logLineStart);

            var productStoreRequests = new List<ProductStoreRequest>();
            var url = $"{BaseUrl}/zoeken/api/products/search?taxonomySlug={slug}&size=10000";

            try
            {
                using (var httpResponse = await _client.GetAsync(url))
                using (var stream = await httpResponse.Content.ReadAsStreamAsync())
                using (var data = JsonDocument.Parse(stream))
                {
                    var taxonomies = data.RootElement.GetProperty("aggregation").GetProperty("taxonomies").EnumerateArray().ToList();
                    if (taxonomies.Any())
                    {
                        foreach (var taxonomy in taxonomies)
                        {
                            var logLineTaxonomy = $"Scraping category: {taxonomy.GetProperty("label")}";
                            Console.WriteLine(logLineTaxonomy);
                            _streamWriter.WriteLine(logLineTaxonomy);

                            url = $"{BaseUrl}/zoeken/api/products/search?taxonomySlug={slug}&taxonomy={taxonomy.GetProperty("id")}&size=10000";
                            using var httpResponsetaxonomy = await _client.GetAsync(url);
                            using var streamtaxonomy = await httpResponsetaxonomy.Content.ReadAsStreamAsync();
                            using var dataTaxonomy = JsonDocument.Parse(streamtaxonomy);
                            foreach (var cards in dataTaxonomy.RootElement.GetProperty("cards").EnumerateArray())
                            {
                                foreach (var productData in cards.GetProperty("products").EnumerateArray())
                                {
                                    var productStoreRequest = new ProductStoreRequest
                                    {
                                        Code = productData.GetProperty("id").ToString(),
                                        Url = BaseUrl + productData.GetProperty("link").ToString(),
                                        IsStoreAdvertisedVegan = productData.GetProperty("properties").GetProperty("lifestyle").EnumerateArray().Any(_ => _.ToString() == "dieet_veganistisch")
                                    };
                                    productStoreRequests.Add(productStoreRequest);
                                }
                            }
                        }
                    }
                }

                var logLineEnd = $"Found {productStoreRequests.Count} products.";
                Console.WriteLine(logLineEnd);
                _streamWriter.WriteLine(logLineEnd);
            }
            catch (Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping: {url}");
                _streamWriter.WriteLine(ex);
            }

            return productStoreRequests;
        }

        async Task HandleProduct(ProductStoreRequest request, ProductCategory productCategory)
        {
            try
            {
                var productDocument = await _browsingContext.OpenAsync(request.Url);

                var productData = JsonDocument.Parse(productDocument.QuerySelector("script[type='application/ld+json']").InnerHtml).RootElement;

                request.StoreType = StoreType.AlbertHeijn;
                request.Name = productData.GetProperty("name").GetString();
                request.Ammount = productData.GetProperty("weight").GetString();
                request.Ingredients = GetIngredients(productDocument);
                request.AllergyInfo = GetAllergyInfo(productDocument);

                var foundImageUrl = productData.TryGetProperty("image", out var imageUrl);
                request.ImageUrl = foundImageUrl ? imageUrl.GetString() : null;

                if (request.IsStoreAdvertisedVegan)
                {
                    request.IsStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(productDocument);
                }

                request.LastScrapeDate = _scrapeDate;
                request.ProductCategory = productCategory;

                _productService.CreateOrUpdate(request);

                var logLine = $"Handled product: {request.Code} {request.Name}";
                Console.WriteLine(logLine); 
                _streamWriter.WriteLine(logLine);
            }
            catch(Exception ex)
            {
                _streamWriter.WriteLine($"Error scraping product: {request.Code}");
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

        private ProductStoreRequest ProductToBareRequest(Product product)
        {
            return new ProductStoreRequest
            {
                Code = product.Code,
                Url = product.Url
            };
        }
    }
}
