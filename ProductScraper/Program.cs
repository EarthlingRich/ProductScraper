using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using ProductScraper.Models;
using ProductScraper.Scrapers;
using ProductScraper.Services;

namespace ProductScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>();

            using (var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = GetProductScraper(args[0], driver);
                if (args.Length == 2) {
                    products.AddRange(scraper.ScrapeCategory(args[1]));
                }
                else {
                    products.AddRange(scraper.ScrapeAll());
                }
            }

            var productService = new ProductService();
            foreach (Product product in products) {
                productService.Create(product);
            }
        }

        static IProductScraper GetProductScraper(string store, ChromeDriver driver) {
            Enum.TryParse(store, out StoreType storeType);
            switch (storeType) {
                case StoreType.AlbertHeijn:
                    return new AlbertHeijnScraper(driver);
                default:
                    throw new ArgumentException("Product scraper for store not found.");
            }
        }
    }
}
