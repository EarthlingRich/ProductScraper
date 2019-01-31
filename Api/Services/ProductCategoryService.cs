﻿using System.Linq;
using Api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace Api.Services
{
    public class ProductCategoryService
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public ProductCategoryService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ProductCategory Create(ProductCategoryCreateRequest request)
        {
            var productCategory = _mapper.Map<ProductCategory>(request);
            _context.ProductCategories.Add(productCategory);
            _context.SaveChanges();

            return productCategory;
        }

        public void Update(ProductCategoryUpdateRequest request)
        {
            var productCategory = _context.ProductCategories.Find(request.Id);
            _mapper.Map(request, productCategory);
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var productCategory = _context.ProductCategories.Find(id);
            var productsWithCategory = _context.Products
                    .Include("ProductIngredients.Ingredient")
                    .Where(_ => _.ProductProductCategories.Any(c => c.ProductCategory == productCategory));

            if (!productsWithCategory.Any())
            {
                _context.ProductCategories.Remove(productCategory);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
