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

        public IngredientController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _ingredientService = new IngredientService(_mapper, _unitOfWork);
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

        public IActionResult Update(int id)
        {
            var ingredient = _unitOfWork.Ingredients.Get(id);
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
