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
    public class ProductController : Controller
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;
        readonly ProductService _productService;

        public ProductController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productService = new ProductService(_context, _mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductList(IDataTablesRequest dataTablesRequest) {
            var products = _context.Products.Skip(dataTablesRequest.Start).Take(dataTablesRequest.Length).ToList();
            var data = products.Select(_ => _mapper.Map<ProductViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, data.Count(), _context.Products.Count(), data);

            return new DataTablesJsonResult(response, true);
        }

        public IActionResult Workload()
        {
            return View();
        }

        public IActionResult WorkloadList(IDataTablesRequest dataTablesRequest)
        {
            var products = _context.Products.Where(_ => !_.IsProcessed).Skip(dataTablesRequest.Start).Take(dataTablesRequest.Length).ToList();
            var data = products.Select(_ => _mapper.Map<ProductViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, data.Count(), _context.Products.Count(), data);

            return new DataTablesJsonResult(response, true);
        }

        public IActionResult Update(int id)
        {
            var product = _context.Products.Include("ProductIngredients.Ingredient").FirstOrDefault(_ => _.Id == id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(ProductViewModel viewModel)
        {
            _productService.Update(viewModel);
            return RedirectToAction("Index");
        }

        public IActionResult Process(int id)
        {
            var product = _productService.ProcessIsVegan(id);
            var viewModel = _mapper.Map<ProductViewModel>(product);

            return View("Update", viewModel);
        }

        public IActionResult ProcessAll()
        {
            _productService.ProcessAllNonVegan();

            return Redirect("Index");
        }
    }
}
