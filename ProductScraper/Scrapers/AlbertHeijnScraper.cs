using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        static readonly string URL = "https://www.ah.nl/producten/";
        readonly ApplicationContext _context;
        readonly ChromeDriver _driver;
        readonly ProductApplicationService _productService;
        readonly DateTime _scrapeDate;
        readonly StreamWriter _streamWriter;

        public AlbertHeijnScraper(ChromeDriver driver, ApplicationContext context, IMapper mapper, StreamWriter streamWriter, DateTime scrapeDate)
        {
            _context = context;
            _driver = driver;
            _streamWriter = streamWriter;
            _scrapeDate = scrapeDate;

            _productService = new ProductApplicationService(_context, mapper);
        }

        public void ScrapeAll()
        {
            var productUrlDictonary = new Dictionary<int, List<string>>();

            //Get main categorie url's
            var productCategories = _context.ProductCategories.Include(_ => _.StoreCategories).Where(_ => _.StoreCategories.Any(sc => sc.StoreType == StoreType.AlbertHeijn));

            //Get product url's from main categories
            foreach (var productCategorie in productCategories)
            {
                var productCategorieUrls = productCategorie.StoreCategories.Select(_ => _.Url);
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

            //Get product data
            foreach (var productsUrlsForCategory in productUrlDictonary)
            {
                var productCateogry = _context.ProductCategories.Find(productsUrlsForCategory.Key);
                var productsUrlsForCategoryDistinct = productsUrlsForCategory.Value.Distinct().ToList();

                foreach (var productUrl in productsUrlsForCategoryDistinct)
                {
                    HandleProduct(productUrl, productCateogry, _driver);
                }
            }

            //Remove outdated products
            _productService.RemoveOutdatedProducts(StoreType.AlbertHeijn, _scrapeDate);
        }

        public void ScrapeCategory(string url)
        {
            var productUrls = new List<string>();
            var productCategorie = _context.ProductCategories.First(_ => _.StoreCategories.Any(sc => sc.Url == url));

            //Get product url's from category
            productUrls.AddRange(GetProductUrls(url, _driver));

            //Get product data
            foreach (string productUrl in productUrls)
            {
                HandleProduct(productUrl, productCategorie, _driver);
            }
        }

        public void ScrapeAllCategories()
        {
            var productCategoryUrls = new List<string>();
            var mainCategoryUrls = GetMainCategories(URL, _driver);

            foreach(var mainCategoryUrl in mainCategoryUrls)
            {
                productCategoryUrls.AddRange(GetProductCategoryUrls(mainCategoryUrl, _driver));
            }

            productCategoryUrls = productCategoryUrls.Distinct().ToList();

            foreach (var productCategoryUrl in productCategoryUrls)
            {
                if (!_context.StoreCategories.Any(_ => _.Url == productCategoryUrl))
                {
                    var storeCategory = new StoreCategory
                    {
                        Url = productCategoryUrl,
                        StoreType = StoreType.AlbertHeijn
                    };
                    _context.StoreCategories.Add(storeCategory);
                    _context.SaveChanges();
                }
            }
        }

        static List<string> GetMainCategories(string url, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            return driver.FindElementsByXPath("//div[contains(@class, 'product-category-navigation-lane')]//a[contains(@class, 'category-link')]")
                         .Select(_ => _.GetAttribute("href"))
                         .ToList();
        }

        static List<string> GetProductCategoryUrls(string url, ChromeDriver driver)
        {
            var productCategoryUrls = new List<string>();

            driver.Navigate().GoToUrl(url);

            //Scroll to footer to load all categories and products
            var footer = driver.FindElementByXPath("//footer");
            Actions actions = new Actions(driver);
            actions.MoveToElement(footer);
            actions.Perform();

            //Get product category url's, keep try loading sub categories unitl no categories are found
            var productCategoryUrlsForPage = driver.FindElementsByXPath("//div[contains(@class, 'product-lane')]//div[contains(@class, 'legend')]//a[contains(@class, 'grid-item__content')]")
                                            .Select(_ => _.GetAttribute("href"))
                                            .Where(_ => _.StartsWith("https://www.ah.nl/producten/", StringComparison.InvariantCulture))
                                            .ToList();

            productCategoryUrls.AddRange(productCategoryUrlsForPage);

            if (productCategoryUrlsForPage.Any())
            {
                foreach (string productCategoryUrl in productCategoryUrlsForPage)
                {
                    productCategoryUrls.AddRange(GetProductUrls(productCategoryUrl, driver));
                }
            }

            return productCategoryUrls;
        }

        static List<string> GetProductUrls(string url, ChromeDriver driver)
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

            //If there are no sub categories found load get product url's
            productUrls.AddRange(driver.FindElementsByXPath("//a[contains(@class, 'product__content--link')]")
                                 .Select(_ => _.GetAttribute("href"))
                                 .ToList());
            return productUrls;
        }

        void HandleProduct(string url, ProductCategory productCategory, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var ingredients = GetIngredients(driver);
            var allergyInfo = GetAllergyInfo(driver);
            var isStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(driver);

            try
            {
                var request = new ProductStoreRequest
                {
                    StoreType = StoreType.AlbertHeijn,
                    Name = driver.FindElementByXPath("//h1[contains(@class, 'product-description__title')]").Text,
                    Url = url,
                    Ingredients = ingredients,
                    AllergyInfo = allergyInfo,
                    IsStoreAdvertisedVegan = isStoreAdvertisedVegan,
                    LastScrapeDate = _scrapeDate,
                    ProductCategory = productCategory
                };

                _productService.CreateOrUpdate(request);
            }
            catch(Exception ex)
            {
                _streamWriter.WriteLine($"Error getting product: { driver.Url }");
                _streamWriter.WriteLine(ex);
            }
        }

        private string GetIngredients(ChromeDriver driver) {
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
                ingredients = driver.FindElementByXPath("//h1[@id='ingredienten']/following-sibling::p").Text.ToLower();
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

        private string GetAllergyInfo(ChromeDriver driver)
        {
            var replaceRegex = new List<string> {
                @"bevat: ",
                @"kan bevatten.*"
            };

            var allergyInfo = "";
            try
            {
                allergyInfo = driver.FindElementByXPath("//h2[@id='allergie-informatie']/following-sibling::p").Text.ToLower();
            }
            catch
            {
                return allergyInfo;
            }

            foreach (var regex in replaceRegex)
            {
                allergyInfo = Regex.Replace(allergyInfo, regex, "");
            }

            return allergyInfo.Trim();
        }

        private bool GetIsStoreAdvertisedVegan(ChromeDriver driver)
        {
            var isVegan = driver.PageSource.Contains("icon--ah_vegan");

            if (!isVegan)
            {
                isVegan = driver.FindElementsByXPath("//p[contains(@class, 'product__summary')]//*[contains(text(), 'vegan') or contains(text(), 'Vegan')]").Any();
            }

            return isVegan;
        }
    }
}
