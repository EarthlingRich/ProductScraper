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
        readonly DateTime _productActivityDate;

        public ProductApplicationService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _productActivityDate = DateTime.Now;
        }

        private void Create(ProductStoreRequest request)
        {
            var product = _mapper.Map<Product>(request);
            product.VeganType = VeganType.Unkown;
            product.ProductCategories.Add(request.ProductCategory);

            _context.Products.Add(product);

            var workloadItemNew = new WorkloadItem
            {
                Message = "Nieuw product gevonden",
                CreatedOn = product.LastScrapeDate
            };
            product.WorkloadItems.Add(workloadItemNew);

            if (product.IsStoreAdvertisedVegan)
            {
                var workloadItemVegan = new WorkloadItem
                {
                    Message = "Product is wel vegan volgens winkel",
                    CreatedOn = request.LastScrapeDate
                };
                product.WorkloadItems.Add(workloadItemVegan);
            }

            _context.SaveChanges();
        }

        public void Update(ProductUpdateRequest request)
        {
            var product = _context.Products
                    .Include(p => p.WorkloadItems)
                    .Single(_ => _.Id == request.Id);

            var oldVeganType = product.VeganType;

            _mapper.Map(request, product);

            if (oldVeganType != product.VeganType)
            {
                product.AddProductActivityVeganTypeChanged(_productActivityDate);
            }

            _context.SaveChanges();
        }

        private void Update(ProductStoreRequest request)
        {
            var product = _context.Products
                    .Include(p => p.WorkloadItems)
                    .Include(p => p.ProductProductCategories)
                    .Single(_ => _.Url == request.Url);

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

            if (product.IsStoreAdvertisedVegan != request.IsStoreAdvertisedVegan)
            {
                var workloadItem = new WorkloadItem
                {
                    Product = product,
                    Message = $"Product is { (request.IsStoreAdvertisedVegan ? "wel" : "niet")} vegan volgens winkel",
                    CreatedOn = request.LastScrapeDate
                };
                _context.WorkloadItems.Add(workloadItem);
            }

            _mapper.Map(request, product);

            if (!product.ProductCategories.Contains(request.ProductCategory))
            {
                product.ProductCategories.Add(request.ProductCategory);
            }

            _context.SaveChanges();
        }

        public void CreateOrUpdate(ProductStoreRequest request)
        {
            var existingProduct = _context.Products.Any(_ => _.Url == request.Url);
            if (!existingProduct)
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

        public void ProcessAll()
        {
            var productIds = _context.Products.Select(_ => _.Id);
            var ingredients = _context.Ingredients.ToList();

            foreach(var productId in productIds)
            {
                var product = _context.Products
                        .Include(p => p.WorkloadItems)
                        .Include(p => p.ProductActivities)
                        .Include("ProductIngredients.Ingredient")
                        .Single(_ => _.Id == productId);

                SetMatchedIngredients(product, ingredients);
                SetVeganType(product);

                if (product.IsProcessed)
                {
                    var workloadItems = product.WorkloadItems.Where(_ => !_.IsProcessed);
                    foreach (var workloadItem in workloadItems)
                    {
                        workloadItem.IsProcessed = true;
                    }
                }

                _context.SaveChanges();
            }
        }

        public Product ProcessVeganType(int productId)
        {
            var product = _context.Products
                    .Include("ProductIngredients.Ingredient")
                    .Single(_ => _.Id == productId);
            var oldVeganType = product.VeganType;
            var ingredients = _context.Ingredients.ToList();

            SetMatchedIngredients(product, ingredients);

            if (product.IsStoreAdvertisedVegan)
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

            if (oldVeganType != product.VeganType)
            {
                product.AddProductActivityVeganTypeChanged(_productActivityDate);
            }

            product.IsProcessed = true;

            _context.SaveChanges();
            return product;
        }

        private void SetMatchedIngredients(Product product, List<Ingredient> ingredients)
        {
            var foundIngredients = new List<Ingredient>();
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
                    foundIngredients.Add(ingredient);
                }
            }

            foreach(var foundIngredient in foundIngredients)
            {
                if (!product.MatchedIngredients.Contains(foundIngredient))
                {
                    product.MatchedIngredients.Add(foundIngredient);
                    product.ProductActivities.Add(new ProductActivity
                    {
                        Type = ProductActivityType.IngredientAdded,
                        Detail = foundIngredient.Name,
                        CreatedOn = _productActivityDate
                    });
                }
            }

            var outdatedIngredients = new List<Ingredient>();
            foreach (var matchedIngredient in product.MatchedIngredients)
            {
                if (!foundIngredients.Contains(matchedIngredient))
                {
                    outdatedIngredients.Add(matchedIngredient);
                }
            }

            foreach (var outdatedIngredient in outdatedIngredients)
            {
                product.MatchedIngredients.Remove(outdatedIngredient);
                product.ProductActivities.Add(new ProductActivity
                {
                    Type = ProductActivityType.IngredientRemoved,
                    Detail = outdatedIngredient.Name,
                    CreatedOn = _productActivityDate
                });
            }
        }

        private void SetVeganType(Product product)
        {
            var oldVeganType = product.VeganType;

            if (product.IsStoreAdvertisedVegan || product.IsManufacturerAdvertisedVegan)
            {
                product.VeganType = VeganType.Vegan;
                product.IsProcessed = true;
            }
            else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Not))
            {
                product.VeganType = VeganType.Not;
                if (!product.MatchedIngredients.Where(_ => _.VeganType == VeganType.Not).All(_ => _.NeedsReview))
                {
                    product.IsProcessed = true;
                }
            }
            else if (product.MatchedIngredients.Any(_ => _.VeganType == VeganType.Unsure))
            {
                product.VeganType = VeganType.Unsure;
                if (!product.MatchedIngredients.Where(_ => _.VeganType == VeganType.Unsure).All(_ => _.NeedsReview))
                {
                    product.IsProcessed = true;
                }
            }

            if (oldVeganType != product.VeganType)
            {
                product.AddProductActivityVeganTypeChanged(_productActivityDate);
            }
        }

        public void RemoveOutdatedProducts(StoreType storeType, DateTime scrapeDate)
        {
            var outdatedProducts = _context.Products.Where(_ => _.StoreType == storeType && _.LastScrapeDate < scrapeDate.AddDays(-7));

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
