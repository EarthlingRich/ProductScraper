using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace Api.Services
{
    public class ProductService
    {
        readonly ApplicationContext _context;

        public ProductService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Product> ProcessMatchedIngredientsForProductAsync(int productId)
        {
            var product = await _context.Products.Include("ProductIngredients.Ingredient").SingleOrDefaultAsync(_ => _.Id == productId);
            var ingredients = _context.Ingredients;

            product.MatchedIngredients.Clear();

            foreach(var ingredient in ingredients)
            {
                foreach(var keyWord in ingredient.KeyWords)
                {
                    var match = Regex.Match(product.Ingredients, @"[\s,]" + keyWord + @"[\s,]");
                    if (match.Success)
                    {
                        product.MatchedIngredients.Add(ingredient);
                        break;
                    }
                }
            }

            _context.SaveChanges();
            return product;
        }
    }
}
