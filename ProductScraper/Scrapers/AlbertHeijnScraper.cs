using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace ProductScraper.Scrapers
{
    public class AlbertHeijnScraper : IProductScraper
    {
        readonly ApplicationContext _context;
        readonly ChromeDriver _driver;
        readonly ProductApplicationService _productService;
        readonly DateTime _scrapeDate;
        readonly StreamWriter _streamWriter;
        readonly IBrowsingContext _browsingContext;

        public AlbertHeijnScraper(ChromeDriver driver, ApplicationContext context, IMapper mapper, StreamWriter streamWriter, DateTime scrapeDate)
        {
            _context = context;
            _driver = driver;
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
                        productUrlDictonary[productCategorie.Id].AddRange(GetProductUrls(productCategorieUrl, _driver));
                    }
                    else
                    {
                        productUrlDictonary.Add(productCategorie.Id, GetProductUrls(productCategorieUrl, _driver));
                    }
                }
            }

            //Lower scraping wait time
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

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
                productUrls.AddRange(GetProductUrls(productCategorieUrl, _driver));
            }

            //Lower scraping wait time
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //Get product data
            foreach (string productUrl in productUrls)
            {
                await HandleProduct(productUrl, productCategorie);
            }
        }

        List<string> GetProductUrls(string url, ChromeDriver driver)
        {
            var productUrls = new List<string>();

            driver.Navigate().GoToUrl(url);

            //Scroll to footer to load all categories and products
            var footer = driver.FindElementByXPath("//footer");
            Actions actions = new Actions(driver);
            actions.MoveToElement(footer);
            actions.Perform();

            //Get product category url's, keep try loading sub categories unitl no categories are found
            var productCategoryUrls = driver.FindElementsByXPath("//div[contains(@class, 'product-lane')]//div[contains(@class, 'legend')]//a[contains(@class, 'grid-item__content')]")
                                            .Select(_ => _.GetAttribute("href"))
                                            .Where(_ => _.StartsWith("https://www.ah.nl/producten/", StringComparison.InvariantCulture))
                                            .ToList();
            productCategoryUrls.AddRange(driver.FindElementsByXPath("//article[contains(@class, 'see-more-lane')]//div[contains(@class, 'see-more--entry')]//a[contains(@class, 'grid-item__content')]")
                                .Select(_ => _.GetAttribute("href"))
                                .Where(_ => _.StartsWith("https://www.ah.nl/producten/", StringComparison.InvariantCulture))
                                .ToList());
            if (productCategoryUrls.Any())
            {
                foreach (string productCategoryUrl in productCategoryUrls)
                {
                    productUrls.AddRange(GetProductUrls(productCategoryUrl, driver));
                }
            }

            //If there are no sub categories found get product url's
            productUrls.AddRange(driver.FindElementsByXPath("//a[contains(@class, 'product__content--link')]")
                                 .Select(_ => _.GetAttribute("href"))
                                 .ToList());
            #if DEBUG
                Console.WriteLine($"Scraped category: {url}");
            #endif
            _streamWriter.WriteLine($"Scraped category: {url}");

            return productUrls;
        }

        async Task HandleProduct(string url, ProductCategory productCategory)
        {
            try
            {
                //Force loading old product page, Albert Heijn is testing with a new design.
                url = url.Replace("ah.nl/producten2/product/", "ah.nl/producten/product/");
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

                #if DEBUG
                    Console.WriteLine($"Handled product: {name}");
                #endif
                _streamWriter.WriteLine($"Handled product: {name}");
            }
            catch(Exception ex)
            {
                _streamWriter.WriteLine($"Error getting product: { url }");
                _streamWriter.WriteLine(ex);
            }
        }

        private string GetIngredients(IDocument productDocument) {
            var replaceRegex = new List<string> {
                @"ingrediënten:",
                @"dit product.*",
                @"kan sporen.*",
                @"allergiewijzer.*",
                @"allergie info.*",
                @"allergie informatie.*",
                @"allergie-informatie.*",
                @"kan.*bevatten.*"
            };

            var ingredients = "";
            try
            {
                ingredients = productDocument
                    .QuerySelectorAll("div.product-info-ingredients h2")
                    .Where(_ => _.TextContent == "ingrediënten")
                    .First()
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
                    .Where(_ => _.TextContent == "allergie-informatie")
                    .First()
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
                isVegan = summaryNodes.TextContent.ToLower().Contains("vegan");
            }

            return isVegan;
        }
    }
}
