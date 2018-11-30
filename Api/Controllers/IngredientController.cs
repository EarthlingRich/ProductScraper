using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class IngredientController : Controller
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;
        readonly IngredientService _ingredientService;

        public IngredientController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ingredientService = new IngredientService(_unitOfWork);
        }

        public IActionResult Index()
        {
            var ingredients = _unitOfWork.Ingredients.GetAll();
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
    }
}
