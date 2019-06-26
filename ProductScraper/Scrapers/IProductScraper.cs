using System.Threading.Tasks;

namespace ProductScraper.Scrapers
{
    public interface IProductScraper
    {
        Task ScrapeAll();
        Task ScrapeCategory(string scrapeCategoryName);
    }
}
