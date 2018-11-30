using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class IngredientController : Controller
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;

        public IngredientController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var ingredients = _unitOfWork.Ingredients.GetAll();
            var viewModel = ingredients.Select(_ => _mapper.Map<IngredientViewModel>(_));

            return View(viewModel);
        }
    }
}
