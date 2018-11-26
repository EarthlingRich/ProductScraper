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

            return View(products);
        }

        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.Products.Get(id);

            return View(product);
        }
    }
}
