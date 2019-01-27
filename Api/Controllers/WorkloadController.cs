using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Api.Controllers
{
    public class WorkloadController : Controller
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public WorkloadController(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductList(IDataTablesRequest dataTablesRequest)
        {
            var productsQuery = _context.Products.Where(_ => _.IsProcessed);
            var totalCount = productsQuery.Count();

            if (dataTablesRequest.Search.Value != null)
            {
                productsQuery = productsQuery.Where(_ => _.Name.Contains(dataTablesRequest.Search.Value));
            }

            var filteredCount = productsQuery.Count();
            var products = productsQuery.Skip(dataTablesRequest.Start).Take(dataTablesRequest.Length).ToList();

            var data = products.Select(_ => _mapper.Map<WorkloadProductListViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, totalCount, filteredCount, data);

            return new DataTablesJsonResult(response, true);
        }
    }
}
