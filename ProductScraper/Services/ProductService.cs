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

        public void Add(Product product)
        {
            _productRepository.Add(product);
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }

        public void UpdateOrAdd(Product product)
        {
            _productRepository.Update(product);

            var existingProduct = GetProductByUrl(product.Url);
            if (existingProduct == null)
            {
                Add(product);
            }
            else
            {
                existingProduct.Name = product.Name;
                Update(existingProduct);
            }
        }

        public Product GetProductByUrl(string url)
        {
            return _productRepository.GetByUrl(url);
        }
    }
}
