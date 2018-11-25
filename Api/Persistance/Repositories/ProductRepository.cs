using Api.Persistance.Interfaces;
using Model;
using Model.Models;

namespace Api.Persistance.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationContext context)
        : base(context){}
    }
}