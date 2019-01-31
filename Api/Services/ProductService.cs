using System.Collections.Generic;
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

        public void Update(ProductUpdateRequest viewModel)
        {
            var product = _context.Products.Find(viewModel.Id);
            _mapper.Map(viewModel, product);
            _context.SaveChanges();
        }

        public void ProcessAllNonVegan()
        {
            var products = _context.Products.Include("ProductIngredients.Ingredient").ToList();
            var ingredients = _context.Ingredients.ToList();

            foreach(var product in products)
            {
                SetMatchedIngredients(product, ingredients);

                if (product.StoreAdvertisedVegan)
                {
                    product.VeganType = VeganType.Vegan;
                }
                else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Not))
                {
                    product.VeganType = VeganType.Not;
                    if (!product.MatchedIngredients.Where(_ => _.VeganType == VeganType.Not).Any(_ => _.NeedsReview))
                    {
                        product.IsProcessed = true;
                    }
                }
                else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Unsure))
                {
                    product.VeganType = VeganType.Unsure;
                    if (!product.MatchedIngredients.Where(_ => _.VeganType == VeganType.Unsure).Any(_ => _.NeedsReview))
                    {
                        product.IsProcessed = true;
                    }
                }
            }

            _context.SaveChanges();
        }

        public Product ProcessVeganType(int productId)
        {
            var product =  _context.Products.Include("ProductIngredients.Ingredient").First(_ => _.Id == productId);
            var ingredients = _context.Ingredients.ToList();

            SetMatchedIngredients(product, ingredients);

            if(product.StoreAdvertisedVegan)
            {
                product.VeganType = VeganType.Vegan;
            }
            else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Not))
            {
                product.VeganType = VeganType.Not;
            }
            else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Unsure))
            {
                product.VeganType = VeganType.Unsure;
            }
            else
            {
                product.VeganType = VeganType.Vegan;
            }

            product.IsProcessed = true;

            _context.SaveChanges();
            return product;
        }

        private void SetMatchedIngredients(Product product, List<Ingredient> ingredients)
        {
            product.MatchedIngredients.Clear();

            foreach (var ingredient in ingredients)
            {
                var foundMatch = false;

                foundMatch = DetectAllergyKeyword(ingredient, product);

                if (!foundMatch)
                {
                    foundMatch = DetectyKeyword(ingredient, product);
                }

                if (foundMatch)
                {
                    product.MatchedIngredients.Add(ingredient);
                }
            }
        }

        private bool DetectAllergyKeyword(Ingredient ingredient, Product product)
        {
            foreach (var keyWord in ingredient.AllergyKeywords)
            {
                var match = Regex.Match(" " + product.AllergyInfo + " ", @"[\s\W]" + keyWord + @"[\s\W]");
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }

        private bool DetectyKeyword(Ingredient ingredient, Product product)
        {
            foreach (var keyWord in ingredient.KeyWords)
            {
                var match = Regex.Match(" " + product.Ingredients + " ", @"[\s\W]" + keyWord + @"[\s\W]");
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
