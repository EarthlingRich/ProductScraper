using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
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
        static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();
            var context = GetApplicationContext(configuration);

            var logPath = configuration["LogPath"];
            Directory.CreateDirectory(logPath);
            var file = File.Create($"{logPath}{args[0]}-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm")}.txt");

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ApplicationMapperConfiguration())
            );
            var mapper = new Mapper(config);

            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            using (var driver = new ChromeDriver(options))
            using (var streamWriter = new StreamWriter(file))
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                var scraper = GetProductScraper(args[0], driver, context, mapper, streamWriter);
                if (args.Length == 2)
                {
                    await scraper.ScrapeCategory(args[1]);
                }
                else
                {
                    await scraper.ScrapeAll();
                }
            }
        }

        static IProductScraper GetProductScraper(string store, ChromeDriver driver, ApplicationContext context, IMapper mapper, StreamWriter streamWriter)
        {
            Enum.TryParse(store, out StoreType storeType);
            switch (storeType)
            {
                case StoreType.AlbertHeijn:
                    return new AlbertHeijnScraper(driver, context, mapper, streamWriter, DateTime.Now);
                case StoreType.Jumbo:
                    return new JumboScraper(driver, context, mapper, streamWriter, DateTime.Now);
                default:
                    throw new ArgumentException("Product scraper for store not found.");
            }
        }

        private static ApplicationContext GetApplicationContext(IConfigurationRoot configuration)
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

           return new ApplicationContext(builder.Options);
        }
    }
}
