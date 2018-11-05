namespace ProductScraper.Scrapers
{
    public interface IProductScraper
    {
        void ScrapeAll();
        void ScrapeCategory(string url);
    }
}
