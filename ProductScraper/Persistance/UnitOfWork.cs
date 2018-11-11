using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using ProductScraper.Persistance.Interfaces;
using ProductScraper.Persistance.Interfaces.Repositories;
using ProductScraper.Persistance.Repositories;

namespace ProductScraper.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationContext _context;

        public UnitOfWork()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            _context = new ApplicationContext(builder.Options);
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
