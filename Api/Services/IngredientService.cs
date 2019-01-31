using System.Linq;
using Api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace Api.Services
{
    public class IngredientService
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public IngredientService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Ingredient Create(IngredientCreateRequest request)
        {
            var ingredient = _mapper.Map<Ingredient>(request);
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();

            return ingredient;
        }

        public void Update(IngredientUpdateRequest request)
        {
            var ingredient = _context.Ingredients.Find(request.Id);
            _mapper.Map(request, ingredient);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            var productsWithIngredient = _context.Products
                    .Include("ProductProductCategories.ProductCategory")
                    .Where(_ => _.ProductIngredients.Any(i => i.Ingredient == ingredient));

            if (!productsWithIngredient.Any())
            {
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
