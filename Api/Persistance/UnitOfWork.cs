using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using Api.Persistance.Interfaces;
using Api.Persistance.Interfaces.Repositories;
using Api.Persistance.Repositories;

namespace Api.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationContext _context;

        public UnitOfWork(ApplicationContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
            Ingredients = new IngredientRepository(_context);
        }

        public IProductRepository Products { get; private set; }
        public IIngredientRepository Ingredients { get; private set; }

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
