using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

namespace ProductScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>();
            var productsUrls = new List<string>();

            var url = "https://www.ah.nl/producten/ontbijtgranen-broodbeleg-tussendoor/"; //Test product category with sub categories url

            using (var driver = new ChromeDriver())
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                productsUrls.AddRange(GetProductsUrls(url, driver));

                foreach(string productUrl in productsUrls) {
                    products.Add(GetProduct(productUrl, driver));
                }
            }
        }

        static List<string> GetProductsUrls(string url, ChromeDriver driver) {
            var productUrls = new List<string>();
            driver.Navigate().GoToUrl(url);

            //Scroll to footer to load all categories and products
            var footer = driver.FindElementByXPath("//footer");
            Actions actions = new Actions(driver);
            actions.MoveToElement(footer);
            actions.Perform();

            //Get product category url's, keep try loading sub categories unitl no categories are found
            var productCategoryUrls = driver.FindElementsByXPath("//div[contains(@class, 'product-lane')]//div[contains(@class, 'legend')]//a[contains(@class, 'grid-item__content')]").Select(_ => _.GetAttribute("href")).ToList();
            if (productCategoryUrls.Any()) {
                foreach (string productCategoryUrl in productCategoryUrls)
                {
                    productUrls.AddRange(GetProductsUrls(productCategoryUrl, driver));
                }
            }

            //If there are no sub categories found load get product url's
            productUrls.AddRange(driver.FindElementsByXPath("//a[contains(@class, 'product__content--link')]").Select(_ => _.GetAttribute("href")).ToList());
            return productUrls;
        }

        static Product GetProduct(string url, ChromeDriver driver) {
            driver.Navigate().GoToUrl(url);

            var product = new Product
            {
                Name = driver.FindElementByXPath("//h1[contains(@class, 'product-description__title')]").Text
            };

            return product;
        }
    }
}
