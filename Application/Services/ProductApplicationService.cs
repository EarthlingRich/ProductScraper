using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;
using Model.Requests;

namespace Application.Services
{
    public class ProductApplicationService
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public ProductApplicationService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private void Create(ProductStoreRequest request)
        {
            var product = _mapper.Map<Product>(request);
            product.IsNew = true;
            product.VeganType = VeganType.Unkown;

            _context.Products.Add(product);

            var workloadItem = new WorkloadItem
            {
                Product = product,
                Message = "Nieuw product gevonden",
                CreatedOn = product.LastScrapeDate
            };
            _context.WorkloadItems.Add(workloadItem);

            _context.SaveChanges();
        }

        public void Update(ProductUpdateRequest request)
        {
            var product = _context.Products.Find(request.Id);
            _mapper.Map(request, product);
            _context.SaveChanges();
        }

        public void Update(ProductStoreRequest request)
        {
            var product = _context.Products.Single(_ => _.Url == request.Url);

            if (request.Ingredients != product.Ingredients || request.AllergyInfo != product.AllergyInfo)
            {
                var workloadItem = new WorkloadItem
                {
                    Product = product,
                    Message = "Product heeft aanpassingen",
                    CreatedOn = request.LastScrapeDate
                };
                _context.WorkloadItems.Add(workloadItem);
            }

            if (product.StoreAdvertisedVegan != request.StoreAdvertisedVegan)
            {
                var workloadItem = new WorkloadItem
                {
                    Product = product,
                    Message = $"Product is { (product.StoreAdvertisedVegan ? "wel" : "niet")} vegan volgens winkel",
                    CreatedOn = request.LastScrapeDate
                };
                _context.WorkloadItems.Add(workloadItem);
            }

            _mapper.Map(request, product);

            _context.SaveChanges();
        }

        public void CreateOrUpdate(ProductStoreRequest request)
        {
            var existingProduct = _context.Products.FirstOrDefault(_ => _.Url == request.Url);
            if (existingProduct == null)
            {
                Create(request);
            }

            Update(request);
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            _context.Products.Remove(product);
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
                    foundMatch = DetectKeyword(ingredient, product);
                }

                if (foundMatch)
                {
                    product.MatchedIngredients.Add(ingredient);
                }
            }
        }

        public void RemoveOutdatedProducts(StoreType storeType, DateTime scrapeDate)
        {
            var outdatedProducts = _context.Products.Where(_ => _.StoreType == storeType && _.LastScrapeDate < scrapeDate);

            foreach (var outdatedProduct in outdatedProducts)
            {
                var workloadItem = new WorkloadItem
                {
                    Product = outdatedProduct,
                    Message = "Product niet gevonden",
                    CreatedOn = scrapeDate
                };
                _context.WorkloadItems.Add(workloadItem);
            }

            _context.SaveChanges();
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

        private bool DetectKeyword(Ingredient ingredient, Product product)
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
