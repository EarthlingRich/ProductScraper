using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;
using ProductScraper.Scrapers;

namespace ProductScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>();

            using (var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = new AlbertHeijnScraper(driver);
                products.AddRange(scraper.ScrapeAllProducts());
            }
        }
    }
}
