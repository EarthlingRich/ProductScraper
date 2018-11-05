using System;
using OpenQA.Selenium.Chrome;
using ProductScraper.Models;
using ProductScraper.Scrapers;

namespace ProductScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = GetProductScraper(args[0], driver);
                if (args.Length == 2) {
                    scraper.ScrapeCategory(args[1]);
                }
                else {
                    scraper.ScrapeAll();
                }
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
