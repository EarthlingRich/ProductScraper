using ProductScraper.Models;
using ProductScraper.Persistance;
using ProductScraper.Persistance.Interfaces.Repositories;

namespace ProductScraper.Services
{
    public class ProductService
    {
        readonly IUnitOfWork _unitOfWork;

        public ProductService() {
            _unitOfWork = new UnitOfWork(new ApplicationContext());
        }

        public void Add(Product product)
        {
            _unitOfWork.Products.Add(product);
            _unitOfWork.Complete();
        }

        public void UpdateOrAdd(Product product)
        {
            var existingProduct = _unitOfWork.Products.FirstOrDefault(_ => _.Url == product.Url);
            if (existingProduct == null)
            {
                Add(product);
            }
            else
            {
                existingProduct.Name = product.Name;
                existingProduct.Ingredients = product.Ingredients;
                _unitOfWork.Complete();
            }
        }
    }
}
