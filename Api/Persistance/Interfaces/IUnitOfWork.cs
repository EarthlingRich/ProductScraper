using System;

namespace Api.Persistance.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IIngredientRepository Ingredients { get; }
        int Complete();
    }
}
