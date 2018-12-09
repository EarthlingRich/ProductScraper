using System.Linq;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Api.Controllers
{
    public class HomeController : Controller
    {
        readonly ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel
            {
                TotalProduccs = _context.Products.Count(),
                TotalVeganProducts = _context.Products.Where(_ => _.IsVegan).Count(),
                TotalWorkload = _context.Products.Where(_ => !_.IsProcessed).Count()
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
