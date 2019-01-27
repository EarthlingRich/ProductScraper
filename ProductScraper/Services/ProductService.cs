using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Models;

namespace ProductScraper.Services
{
    public class ProductService
    {
        readonly ApplicationContext _context;
        readonly StreamWriter _streamWriter;
        readonly DateTime _scrapeDate;

        public ProductService(ApplicationContext context, StreamWriter streamWriter) {
            _context = context;
            _streamWriter = streamWriter;
            _scrapeDate = DateTime.Now;
        }

        public void Add(Product product)
        {
            product.IsNew = true;
            product.VeganType = VeganType.Unkown;

            if (product.StoreAdvertisedVegan)
            {
                product.IsProcessed = true;
            }

            _context.Products.Add(product);

            var workloadItem = new WorkloadItem
            {
                Product = product,
                Message = "Nieuw product gevonden",
                CreatedOn = _scrapeDate
            };
            _context.WorkloadItems.Add(workloadItem);
            _context.SaveChanges();

            _streamWriter.WriteLine($"{product.Id}: Nieuw product {product.Name}");
        }

        public void UpdateOrAdd(Product product)
        {
            var existingProduct = _context.Products.Include("ProductProductCategories.ProductCategory").FirstOrDefault(_ => _.Url == product.Url);
            if (existingProduct == null)
            {
                Add(product);
            }
            else
            {
                //Change product name
                existingProduct.Name = product.Name;

                //Change ingredients and allergy info
                if (existingProduct.Ingredients != product.Ingredients
                    || existingProduct.AllergyInfo != product.AllergyInfo)
                {
                    existingProduct.Ingredients = product.Ingredients;
                    existingProduct.AllergyInfo = product.AllergyInfo;
                    existingProduct.IsProcessed = false;

                    var workloadItem = new WorkloadItem
                    {
                        Product = existingProduct,
                        Message = "Product heeft aanpassingen",
                        CreatedOn = _scrapeDate
                    };
                    _context.WorkloadItems.Add(workloadItem);
                    _streamWriter.WriteLine($"{existingProduct.Id}: Product aangepast {existingProduct.Name}");
                }

                if (existingProduct.StoreAdvertisedVegan != product.StoreAdvertisedVegan)
                {
                    existingProduct.StoreAdvertisedVegan = product.StoreAdvertisedVegan;
                    existingProduct.IsProcessed = product.StoreAdvertisedVegan;

                    var workloadItem = new WorkloadItem
                    {
                        Product = existingProduct,
                        Message = $"Product is { (product.StoreAdvertisedVegan ? "wel" : "niet")} vegan volgens winkel",
                        CreatedOn = _scrapeDate
                    };
                    _context.WorkloadItems.Add(workloadItem);
                    _streamWriter.WriteLine($"{existingProduct.Id}: Product StoreAdvertisedVegan aangepast {existingProduct.Name}");
                }

                //Add new product categorie
                if (!existingProduct.ProductCategories.Contains(product.ProductCategories.First()))
                {
                    existingProduct.ProductCategories.Add(product.ProductCategories.First());
                    var workloadItem = new WorkloadItem
                    {
                        Product = existingProduct,
                        Message = "Product heeft nieuwe categorie",
                        CreatedOn = _scrapeDate
                    };
                    _context.WorkloadItems.Add(workloadItem);
                    _streamWriter.WriteLine($"{existingProduct.Id}: Product heeft nieuwe categorie {existingProduct.Name}");
                }

                _context.SaveChanges();
            }
        }
    }
}
