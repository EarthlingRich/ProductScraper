using System;
using System.Collections.Generic;
using OpenQA.Selenium.Chrome;

namespace ProductScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>();

            var url = "https://www.ah.nl/producten/product/wi132400/ah-extra-jam-aardbeien"; //Test product url

            using(var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
                driver.Navigate().GoToUrl(url);

                var product = new Product();
                product.Name = driver.FindElementByXPath("//h1[contains(@class, 'product-description__title')]").Text;
                products.Add(product);
            }
        }
    }
}
