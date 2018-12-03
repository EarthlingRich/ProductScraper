using System.Linq;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductController : Controller
    {
        readonly IMapper _mapper;
        readonly IUnitOfWork _unitOfWork;
        readonly ProductService _productService;

        public ProductController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productService = new ProductService(_mapper, _unitOfWork);
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Products.GetAll();
            var viewModel = products.Select(_ => _mapper.Map<ProductViewModel>(_));

            return View(viewModel);
        }

        public IActionResult Update(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }

        public IActionResult Process(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            product = _productService.ProcessMatchedIngredientsForProduct(product);
            _unitOfWork.Complete();

            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View("Update", viewModel);
        }
    }
}
