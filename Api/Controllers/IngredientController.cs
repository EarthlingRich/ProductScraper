using System.Linq;
using Api.Models;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Api.Controllers
{
    public class IngredientController : Controller
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;
        readonly IngredientService _ingredientService;

        public IngredientController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _ingredientService = new IngredientService(_context, _mapper);
        }

        public IActionResult Index()
        {
            var ingredients = _context.Ingredients.ToList();
            var viewModel = ingredients.Select(_ => _mapper.Map<IngredientListViewModel>(_));

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new IngredientCreateViewModel());
        }

        [HttpPost]
        public IActionResult Create(IngredientCreateViewModel viewmodel)
        {
            var ingredient = _ingredientService.Create(viewmodel.Request);
            return RedirectToAction("Update", new { id = ingredient.Id });
        }

        public IActionResult Update(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            var viewModel = IngredientUpdateViewModel.Map(ingredient, _mapper);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(IngredientUpdateViewModel viewModel)
        {
            _ingredientService.Update(viewModel.Request);
            return Update(viewModel.Request.Id);
        }
    }
}
