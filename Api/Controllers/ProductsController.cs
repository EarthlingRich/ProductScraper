using System.Collections.Generic;
using System.Linq;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public ProductsController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/products
        [HttpGet]
        public IEnumerable<ProductApiViewModel> Get()
        {
            var products = _context.Products.AsNoTracking().ToList();
            var data = products.Select(_ => _mapper.Map<ProductApiViewModel>(_));

            return data;
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public ProductApiViewModel Get(int id)
        {
            var product = _context.Products.AsNoTracking().First(_ => _.Id == id);
            var data = _mapper.Map<ProductApiViewModel>(product);

            return data;
        }
    }
}
