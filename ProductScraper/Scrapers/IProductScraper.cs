using System.Threading.Tasks;

namespace ProductScraper.Scrapers
{
    public interface IProductScraper
    {
        Task ScrapeAll();
        Task ScrapeProduct(int id);
    }
}
