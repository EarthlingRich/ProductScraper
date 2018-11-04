using ProductScraper.Models;
using ProductScraper.Repositories;

namespace ProductScraper.Services
{
    public class ProductService
    {
        readonly ProductRepository _productRepository;

        public ProductService()
        {
            _productRepository = new ProductRepository();
        }

        public void Create(Product product)
        {
            _productRepository.Create(product);
        }
    }
}
