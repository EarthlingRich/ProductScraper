using System.Linq;
using Api.Models;
using Application.Services;
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
        public static readonly string RouteName = nameof(ProductController).Replace("Controller", "");
        readonly IMapper _mapper;
        readonly ApplicationContext _context;
        readonly ProductApplicationService _productService;

        public ProductController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productService = new ProductApplicationService(_context, _mapper);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductList(IDataTablesRequest dataTablesRequest)
        {
            var productsQuery = _context.Products.Include("ProductProductCategories.ProductCategory");
            var totalCount = productsQuery.Count();

            if (dataTablesRequest.Search.Value != null)
            {
                productsQuery = productsQuery.Where(_ => _.Name.Contains(dataTablesRequest.Search.Value));
            }

            var filteredCount = productsQuery.Count();
            var products = productsQuery.Skip(dataTablesRequest.Start).Take(dataTablesRequest.Length).ToList();

            var data = products.Select(_ => _mapper.Map<ProductListViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, totalCount, filteredCount, data);

            return new DataTablesJsonResult(response, true);
        }

        public IActionResult Update(int id)
        {
            var product = _context.Products
                    .Include(_ => _.WorkloadItems)
                    .Include(_ => _.ProductActivities)
                    .Include("ProductIngredients.Ingredient")
                    .Include("ProductProductCategories.ProductCategory")
                    .First(_ => _.Id == id);
            var viewModel = ProductUpdateViewModel.Map(product, _mapper);

            return View("Update", viewModel);
        }

        [HttpPost]
        public IActionResult Update(ProductUpdateViewModel viewModel)
        {
            _productService.Update(viewModel.Request);
            return RedirectToAction("Index");
        }

        public IActionResult Process(int id)
        {
            _productService.ProcessVeganType(id);
            return Update(id);
        }

        public IActionResult ProcessAll()
        {
            _productService.ProcessAll();

            return Redirect("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _productService.Delete(id);
            return Redirect("Index");
        }
    }
}
