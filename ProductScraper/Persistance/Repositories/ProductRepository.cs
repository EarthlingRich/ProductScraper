using ProductScraper.Models;
using ProductScraper.Persistance.Interfaces;

namespace ProductScraper.Persistance.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationContext context)
        : base(context){}
    }
}