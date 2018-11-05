using ProductScraper.Models;
using ProductScraper.Persistance;

namespace ProductScraper.Services
{
    public class ProductService
    {
        public void Add(Product product)
        {
            using (var unitOfWork = new UnitOfWork(new ApplicationContext()))
            {
                unitOfWork.Products.Add(product);
            }
        }

        public void UpdateOrAdd(Product product)
        {
            using (var unitOfWork = new UnitOfWork(new ApplicationContext()))
            {
                var existingProduct = unitOfWork.Products.FirstOrDefault(_ => _.Url == product.Url);
                if (existingProduct == null)
                {
                    Add(product);
                }
                else
                {
                    existingProduct.Name = product.Name;
                }

                unitOfWork.Complete();
            }
        }
    }
}
