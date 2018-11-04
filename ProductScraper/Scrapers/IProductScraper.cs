using System.Collections.Generic;
using ProductScraper.Models;

namespace ProductScraper.Scrapers
{
    public interface IProductScraper
    {
        IEnumerable<Product> ScrapeAll();
        IEnumerable<Product> ScrapeCategory(string url);
    }
}
