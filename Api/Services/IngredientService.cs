using Api.Models;
using AutoMapper;
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

        public void Create(IngredientViewModel viewModel)
        {
            var ingredient = _mapper.Map<Ingredient>(viewModel);
            _context.Ingredients.Add(ingredient);
            _context.SaveChanges();
        }

        public void Update(IngredientViewModel viewModel)
        {
            var ingredient = _context.Ingredients.Find(viewModel.Id);
            _mapper.Map(viewModel, ingredient);
            _context.SaveChanges();
        }
    }
}
