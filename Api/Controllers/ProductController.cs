using System.Linq;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductController : Controller
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Products.GetAll();
            var viewModel = products.Select(_ => _mapper.Map<ProductViewModel>(_));

            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }
    }
}
