using System.Linq;
using Api.Models;
using Api.Persistance.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class ProductController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Products.GetAll();
            var viewModel = products.Select(_ => new ProductViewModel(_));

            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Get(id);
            var viewModel = new ProductViewModel(product);

            return View(viewModel);
        }
    }
}
