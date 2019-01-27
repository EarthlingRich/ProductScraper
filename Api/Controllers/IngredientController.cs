using System.Linq;
using Api.Models;
using Api.Services;
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
            return View();
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
