using System.Linq;
using Api.Models;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Api.Controllers
{
    public class ProductController : Controller
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;
        readonly ProductService _productService;

        public ProductController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productService = new ProductService(_context);
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            var viewModel = products.Select(_ => _mapper.Map<ProductViewModel>(_));

            return View(viewModel);
        }

        public IActionResult Update(int id)
        {
            var product = _context.Products.Find(id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }

        public IActionResult Process(int id)
        {
            var product = _productService.ProcessMatchedIngredientsForProduct(id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View("Update", viewModel);
        }
    }
}
