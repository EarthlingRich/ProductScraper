using System.Linq;
using Api.Models;
using Api.Resources;
using Application.Services;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Api.Controllers
{
    public class IngredientController : Controller
    {
        public static readonly string RouteName = nameof(IngredientController).Replace("Controller", "");
        readonly IMapper _mapper;
        readonly ApplicationContext _context;
        readonly IngredientApplicationService _ingredientService;

        public IngredientController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _ingredientService = new IngredientApplicationService(_context, _mapper);
        }

        public IActionResult Index()
        {
            return View(nameof(Index));
        }

        public IActionResult IngredientList(IDataTablesRequest dataTablesRequest)
        {
            var ingredientsQuery = _context.Ingredients.AsQueryable();
            var totalCount = ingredientsQuery.Count();

            if (dataTablesRequest.Search.Value != null)
            {
                ingredientsQuery = ingredientsQuery.Where(_ => _.Name.Contains(dataTablesRequest.Search.Value));
            }

            var filteredCount = ingredientsQuery.Count();
            var ingredients = ingredientsQuery
                    .Skip(dataTablesRequest.Start)
                    .Take(dataTablesRequest.Length)
                    .OrderBy(_ => _.Name).ToList();

            var data = ingredients.Select(_ => _mapper.Map<IngredientListViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, totalCount, filteredCount, data);

            return new DataTablesJsonResult(response, true);
        }

        public IActionResult Create()
        {
            return View(nameof(Create), new IngredientCreateViewModel());
        }

        [HttpPost]
        public IActionResult Create(IngredientCreateViewModel viewmodel)
        {
            var ingredient = _ingredientService.Create(viewmodel.Request);
            return RedirectToAction(nameof(Update), new { id = ingredient.Id });
        }

        public IActionResult Update(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            var viewModel = IngredientUpdateViewModel.Map(ingredient, _mapper);

            return View(nameof(Update), viewModel);
        }

        [HttpPost]
        public IActionResult Update(IngredientUpdateViewModel viewModel)
        {
            _ingredientService.Update(viewModel.Request);
            return Update(viewModel.Request.Id);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deleteResult = _ingredientService.Delete(id);

            if (!deleteResult)
            {
                ModelState.AddModelError(string.Empty, CommonTerms.Error_Ingredient_In_Use);
                return Update(id);
            }

            return Redirect(nameof(Index));
        }
    }
}
