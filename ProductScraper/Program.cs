using System;
using System.IO;
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
            var file = File.Create(Environment.CurrentDirectory + $"{args[0]}-{DateTime.Now}.txt");

            using (var driver = new ChromeDriver())
            using (var streamWriter = new StreamWriter(file))
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = GetProductScraper(args[0], driver, context, streamWriter);
                if (args.Length == 2)
                {
                    if(args[1].ToLower() == "-c")
                    {
                        scraper.ScrapeAllCategories();
                    }

                    scraper.ScrapeCategory(args[1]);
                }
                else
                {
                    scraper.ScrapeAll();
                }
            }
        }

        static IProductScraper GetProductScraper(string store, ChromeDriver driver, ApplicationContext context, StreamWriter streamWriter)
        {
            Enum.TryParse(store, out StoreType storeType);
            switch (storeType)
            {
                case StoreType.AlbertHeijn:
                    return new AlbertHeijnScraper(driver, context, streamWriter);
                default:
                    throw new ArgumentException("Product scraper for store not found.");
            }
        }

        private static ApplicationContext GetApplicationContext()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

           return new ApplicationContext(builder.Options);
        }
    }
}
