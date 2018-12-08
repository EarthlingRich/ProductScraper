using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using Model.Models;
using OpenQA.Selenium.Chrome;
using ProductScraper.Scrapers;

namespace ProductScraper
{
    static class Program
    {
        static void Main(string[] args)
        {
            var context = GetApplicationContext();

            using (var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = GetProductScraper(args[0], driver, context);
                if (args.Length == 2)
                {
                    scraper.ScrapeCategory(args[1]);
                }
                else
                {
                    scraper.ScrapeAll();
                }
            }
        }

        static IProductScraper GetProductScraper(string store, ChromeDriver driver, ApplicationContext context)
        {
            Enum.TryParse(store, out StoreType storeType);
            switch (storeType)
            {
                case StoreType.AlbertHeijn:
                    return new AlbertHeijnScraper(driver, context);
                default:
                    throw new ArgumentException("Product scraper for store not found.");
            }
        }

        private static ApplicationContext GetApplicationContext()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

           return new ApplicationContext(builder.Options);
        }
    }
}
