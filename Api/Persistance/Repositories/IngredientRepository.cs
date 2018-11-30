using Api.Persistance.Interfaces;
using Model;
using Model.Models;

namespace Api.Persistance.Repositories
{
    public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(ApplicationContext context)
        : base(context) { }
    }
}
