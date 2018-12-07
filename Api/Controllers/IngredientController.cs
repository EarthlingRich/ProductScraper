using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var viewModel = ingredients.Select(_ => _mapper.Map<IngredientViewModel>(_));

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new IngredientViewModel());
        }

        [HttpPost]
        public IActionResult Create(IngredientViewModel viewmodel)
        {
            _ingredientService.Create(viewmodel);
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var ingredient = _context.Ingredients.Find(id);
            var viewModel = _mapper.Map<IngredientViewModel>(ingredient);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(IngredientViewModel viewModel)
        {
            _ingredientService.Update(viewModel);
            return RedirectToAction("Index");
        }
    }
}
