using System.Linq;
using System.Text.RegularExpressions;
using Api.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace Api.Services
{
    public class ProductService
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public ProductService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Update(ProductViewModel viewModel)
        {
            var product = _context.Products.Find(viewModel.Id);
            _mapper.Map(viewModel, product);
            _context.SaveChanges();
        }

        public Product ProcessMatchedIngredientsForProduct(int productId)
        {
            var product =  _context.Products.Include("ProductIngredients.Ingredient").FirstOrDefault(_ => _.Id == productId);
            var ingredients = _context.Ingredients;

            product.MatchedIngredients.Clear();

            foreach(var ingredient in ingredients)
            {
                var foundMatch = false;

                foreach (var keyWord in ingredient.AllergyKeywords)
                {
                    var match = Regex.Match(product.Ingredients, @"[\s,]*" + keyWord + @"[\s,.]");
                    if (match.Success)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    foreach (var keyWord in ingredient.KeyWords)
                    {
                        var match = Regex.Match(product.Ingredients, @"[\s,]*" + keyWord + @"[\s,.]");
                        if (match.Success)
                        {
                            foundMatch = true;
                            break;
                        }
                    }
                }

                if(foundMatch)
                {
                    product.MatchedIngredients.Add(ingredient);
                }
            }

            product.IsVegan = !product.MatchedIngredients.Any();

            _context.SaveChanges();
            return product;
        }
    }
}
