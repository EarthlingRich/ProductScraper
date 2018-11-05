using System.Linq;
using ProductScraper.Models;

namespace ProductScraper.Repositories
{
    public class ProductRepository
    {
        public void Add(Product product)
        {
            using (var context = new ApplicationContext())
            {
                context.Products.Add(product);
                context.SaveChanges();
            }
        }

        public void Update(Product product)
        {
            using (var context = new ApplicationContext())
            {
                context.Products.Update(product);
                context.SaveChanges();
            }
        }

        public Product GetByUrl(string url)
        {
            using (var context = new ApplicationContext())
            {
                return context.Products.FirstOrDefault(_ => _.Url == url);
            }
        }
    }
}
