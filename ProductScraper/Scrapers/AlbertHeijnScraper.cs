using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Model;
using Model.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using ProductScraper.Services;

namespace ProductScraper.Scrapers
{
    public class AlbertHeijnScraper : IProductScraper
    {
        static readonly string URL = "https://www.ah.nl/producten/";
        readonly ChromeDriver _driver;
        readonly ProductService _productService;

        public AlbertHeijnScraper(ChromeDriver driver, ApplicationContext context)
        {
            _driver = driver;
            _productService = new ProductService(context);
        }

        public void ScrapeAll()
        {
            var productUrls = new List<string>();
            var mainCategoryUrls = new List<string>();

            //Get main categorie url's
            mainCategoryUrls.AddRange(GetMainCategories(URL, _driver));

            //Get product url's from main categories
            foreach (string mainCategoryUrl in mainCategoryUrls)
            {
                productUrls.AddRange(GetProductUrls(mainCategoryUrl, _driver));
            }

            //Remove double products
            productUrls.Distinct();

            //Get product data
            foreach (string productUrl in productUrls)
            {
                HandleProduct(productUrl, _driver);
            }
        }

        public void ScrapeCategory(string url)
        {
            var productUrls = new List<string>();

            //Get product url's from category
            productUrls.AddRange(GetProductUrls(url, _driver));

            //Remove double products
            productUrls.Distinct();

            //Get product data
            foreach (string productUrl in productUrls)
            {
                HandleProduct(productUrl, _driver);
            }
        }

        static List<string> GetMainCategories(string url, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            return driver.FindElementsByXPath("//div[contains(@class, 'product-category-navigation-lane')]//a[contains(@class, 'category-link')]")
                         .Select(_ => _.GetAttribute("href"))
                         .ToList();
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

        void HandleProduct(string url, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var ingredients = GetIngredients(driver);
            var allergyInfo = GetAllergyInfo(driver);

            var product = new Product
            {
                StoreType = StoreType.AlbertHeijn,
                Name = driver.FindElementByXPath("//h1[contains(@class, 'product-description__title')]").Text,
                Url = url,
                Ingredients = ingredients,
                AllergyInfo = allergyInfo
            };

            _productService.UpdateOrAdd(product);
        }

        private string GetIngredients(ChromeDriver driver) {
            var replaceRegex = new List<string> {
                @"ingrediënten: ",
                @"dit product.*",
                @"kan sporen.*",
                @"allergiewijzer.*",
                @"allergie info:.*"
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
    }
}
