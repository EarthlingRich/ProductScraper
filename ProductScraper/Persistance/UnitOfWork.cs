using System;
using ProductScraper.Models;
using ProductScraper.Persistance.Interfaces;
using ProductScraper.Persistance.Interfaces.Repositories;
using ProductScraper.Persistance.Repositories;

namespace ProductScraper.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
        }

        public IProductRepository Products { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
