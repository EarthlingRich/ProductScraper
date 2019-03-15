using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;
using Model.Requests;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ProductScraper.Scrapers
{
    public class JumboScraper : IProductScraper
    {
        readonly ApplicationContext _context;
        readonly ChromeDriver _driver;
        readonly ProductApplicationService _productService;
        readonly DateTime _scrapeDate;
        readonly StreamWriter _streamWriter;

        public JumboScraper(ChromeDriver driver, ApplicationContext context, IMapper mapper, StreamWriter streamWriter, DateTime scrapeDate)
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
                    HandleProduct(productUrl, productCateogry, _driver);
                }
            }

            //Remove outdated products
            _productService.RemoveOutdatedProducts(StoreType.Jumbo, _scrapeDate);
        }

        public void ScrapeCategory(string scrapeCategoryName)
        {
            var productUrls = new List<string>();
            var productCategorie = _context.ProductCategories
                .Include(_ => _.StoreCategories)
                .Single(_ => _.Name.ToLower() == scrapeCategoryName.ToLower());

            //Get product url's from category
            var productCategorieUrls = productCategorie.StoreCategories.Where(_ => _.StoreType == StoreType.Jumbo).Select(_ => _.Url);
            foreach (var productCategorieUrl in productCategorieUrls)
            {
                productUrls.AddRange(GetProductUrls(productCategorieUrl, _driver));
            }

            //Lower scraping wait time
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

            //Get product data
            foreach (string productUrl in productUrls)
            {
                HandleProduct(productUrl, productCategorie, _driver);
            }
        }

        static List<string> GetProductUrls(string url, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            long scrollHeight = 0;
            do
            {
                var js = (IJavaScriptExecutor)driver;
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");
                Thread.Sleep(5000);

                if (newScrollHeight == scrollHeight)
                {
                    break;
                }
                scrollHeight = newScrollHeight;
            }
            while (true);

            var productUrls = driver.FindElementsByXPath("//li[contains(@class, 'jum-result')]//a")
                .Select(_ => _.GetAttribute("href"))
                .ToList();
            var trimmedUrls = new List<string>();

            //Clean url's 
            foreach(var productUrl in productUrls)
            {
                if (productUrl.Contains(";"))
                {
                    trimmedUrls.Add(productUrl.Substring(0, productUrl.IndexOf(";", StringComparison.Ordinal)));
                }
                else
                {
                    trimmedUrls.Add(productUrl);
                }
            }

            return trimmedUrls;
        }

        void HandleProduct(string url, ProductCategory productCategory, ChromeDriver driver)
        {
            driver.Navigate().GoToUrl(url);

            var ingredients = GetIngredients(driver);
            var allergyInfo = GetAllergyInfo(driver);
            var isStoreAdvertisedVegan = GetIsStoreAdvertisedVegan(driver);

            var code = "";
            var codeMatch = Regex.Match(url, @"(?:https?:\/\/www\.jumbo.com\/[^\/.]*\/)(\w*)");
            if (codeMatch.Success)
            {
                code = codeMatch.Groups[1].Value;
            }

            try
            {
                if (code == "")
                {
                    throw new ArgumentException("Product code is empty");
                }

                var request = new ProductStoreRequest
                {
                    StoreType = StoreType.Jumbo,
                    Name = driver.FindElementByXPath("//h1[@data-dynamic-block-name='Title']").Text,
                    Code = code,
                    Url = url,
                    Ingredients = ingredients,
                    AllergyInfo = allergyInfo,
                    IsStoreAdvertisedVegan = isStoreAdvertisedVegan,
                    LastScrapeDate = _scrapeDate,
                    ProductCategory = productCategory
                };

                _productService.CreateOrUpdate(request);
            }
            catch (Exception ex)
            {
                _streamWriter.WriteLine($"Error getting product: { driver.Url }");
                _streamWriter.WriteLine(ex);
            }
        }

        private string GetIngredients(ChromeDriver driver)
        {
            var ingredients = "";
            try
            {
                var elements = driver.FindElementsByXPath("//div[contains(@class, 'jum-ingredients-info')]//h3[contains(text(), 'Ingrediënten')]//following-sibling::ul//li");
                ingredients = string.Join(", ", elements.Select(_ => _.Text.Trim().ToLower()));
            }
            catch
            {
                return ingredients;
            }

            return ingredients;
        }

        private string GetAllergyInfo(ChromeDriver driver)
        {
            var allergyInfo = "";
            try
            {
                var elements = driver.FindElementsByXPath("//div[contains(@class, 'jum-product-allergy-info')]//h3[contains(text(), 'Allergiewaarschuwing')]//following-sibling::ul//li");
                allergyInfo = string.Join(", ", elements.Where(_ => _.Text.ToLower().StartsWith("bevat", StringComparison.Ordinal)).Select(_ => _.Text.Replace("bevat", "").Trim().ToLower()));
            }
            catch
            {
                return allergyInfo;
            }

            return allergyInfo;
        }

        private bool GetIsStoreAdvertisedVegan(ChromeDriver driver)
        {
            return driver.FindElementsByXPath("//div[contains(@class, 'jum-product-info-item')]//*[contains(text(), 'vegan') or contains(text(), 'Vegan')]").Any(); ;
        }
    }
}
