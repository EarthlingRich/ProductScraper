using System.Linq;
using Model;
using Model.Models;

namespace ProductScraper.Services
{
    public class ProductService
    {
        readonly ApplicationContext _context;

        public ProductService(ApplicationContext context) {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateOrAdd(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(_ => _.Url == product.Url);
            if (existingProduct == null)
            {
                Add(product);
            }
            else
            {
                existingProduct.Name = product.Name;
                existingProduct.Ingredients = product.Ingredients;
                _context.SaveChanges();
            }
        }
    }
}
