using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Api.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        static readonly int SIZE = 50;

        readonly ApplicationContext _context;
        readonly IMapper _mapper;

        public ApiController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("products")]
        public async Task<ApiResponse<ApiProduct>> Products(int page)
        {
            var products = await _context.Products.AsQueryable()
                .Skip(SIZE * page)
                .Take(SIZE)
                .ToListAsync();
            var total = await _context.Products.CountAsync();

            return new ApiResponse<ApiProduct>
            {
                Total = total,
                Pages = total / SIZE,
                Items = _mapper.Map<IEnumerable<ApiProduct>>(products)
            };
        }
    }
}
