using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;
using AutoMapper;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Api.Controllers
{
    public class WorkloadController : Controller
    {
        public static readonly string RouteName = nameof(WorkloadController).Replace("Controller", "");
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
            var workLoadItemsQuery = _context.WorkloadItems.Include("Product").Where(_ => !_.IsProcessed);
            var totalCount = workLoadItemsQuery.Count();

            if (dataTablesRequest.Search.Value != null)
            {
                workLoadItemsQuery = workLoadItemsQuery.Where(_ => _.Product.Name.Contains(dataTablesRequest.Search.Value));
            }

            var filteredCount = workLoadItemsQuery.Count();
            var products = workLoadItemsQuery
                    .OrderBy(_ => _.CreatedOn)
                    .Skip(dataTablesRequest.Start)
                    .Take(dataTablesRequest.Length)
                    .ToList();

            var data = products.Select(_ => _mapper.Map<WorkloadItemListViewModel>(_));

            var response = DataTablesResponse.Create(dataTablesRequest, totalCount, filteredCount, data);

            return new DataTablesJsonResult(response, true);
        }
    }
}
