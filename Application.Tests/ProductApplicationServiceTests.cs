using System;
using System.Linq;
using Application.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Models;
using Model.Requests;

namespace Application.Tests
{
    [TestClass]
    public class ProductApplicationServiceTests
    {
        DbContextOptions<ApplicationContext> _options;
        IMapper _mapper;
        readonly int DefaultCount = 2;

        public void AddTestData(ApplicationContext context)
        {
            var productCategory1 = new ProductCategory
            {
                Id = 100,
                Name = "Product category 1",
            };
            context.ProductCategories.Add(productCategory1);

            var productCategory2 = new ProductCategory
            {
                Id = 101,
                Name = "Product category 2"
            };
            context.ProductCategories.Add(productCategory2);

            var product1 = new Product
            {
                Id = 100,
                Name = "Product 1",
                Url = "Product 1 Url"
            };
            product1.ProductCategories.Add(productCategory1);
            context.Products.Add(product1);

            var product2 = new Product
            {
                Id = 101,
                Name = "Product 2",
                Url = "Product 2 Url"
            };
            product2.ProductCategories.Add(productCategory2);
            context.Products.Add(product2);

            context.SaveChanges();
        }

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "ProductScraper" + Guid.NewGuid())
                .Options;
                
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ApplicationMapperConfiguration())
            );
            _mapper = new Mapper(config);
        }

        [TestMethod]
        public void CreateOrUpdate_Create_Valid()
        {
            // Arrange
            ProductStoreRequest request;

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);

                request = new ProductStoreRequest
                {
                    Name = "Product new",
                    StoreType = StoreType.AlbertHeijn,
                    Url = "Product new url",
                    Ingredients = "Ingredients",
                    AllergyInfo = "Allergy info",
                    StoreAdvertisedVegan = true,
                    LastScrapeDate = DateTime.Now,
                    ProductCategory = context.ProductCategories.Find(100)
                };

                // Act
                var productService = new ProductApplicationService(context, _mapper);
                productService.CreateOrUpdate(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Include("ProductProductCategories.ProductCategory").Single(p => p.Name == request.Name);
                Assert.AreEqual(DefaultCount + 1, context.Products.Count());
                Assert.AreEqual(request.Name, product.Name);
                Assert.AreEqual(request.StoreType, product.StoreType);
                Assert.AreEqual(request.Url, product.Url);
                Assert.AreEqual(request.Ingredients, product.Ingredients);
                Assert.AreEqual(request.AllergyInfo, product.AllergyInfo);
                Assert.AreEqual(request.StoreAdvertisedVegan, product.StoreAdvertisedVegan);
                Assert.AreEqual(request.LastScrapeDate, product.LastScrapeDate);
                Assert.AreEqual(request.ProductCategory.Name, product.ProductCategories.First().Name);
                Assert.IsFalse(product.IsProcessed);
            }
        }

        [TestMethod]
        public void CreateOrUpdate_Update_Valid()
        {
            // Arrange
            ProductStoreRequest request;

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);

                request = new ProductStoreRequest
                {
                    Name = "Product new",
                    StoreType = StoreType.AlbertHeijn,
                    Url = "Product 1 Url",
                    Ingredients = "Ingredients",
                    AllergyInfo = "Allergy info",
                    StoreAdvertisedVegan = true,
                    LastScrapeDate = DateTime.Now,
                    ProductCategory = context.ProductCategories.Find(100)
                };

                // Act
                var productService = new ProductApplicationService(context, _mapper);
                productService.CreateOrUpdate(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Include("ProductProductCategories.ProductCategory").Single(p => p.Name == request.Name);
                Assert.AreEqual(DefaultCount, context.Products.Count());
                Assert.AreEqual(request.Name, product.Name);
                Assert.AreEqual(request.StoreType, product.StoreType);
                Assert.AreEqual(request.Url, product.Url);
                Assert.AreEqual(request.Ingredients, product.Ingredients);
                Assert.AreEqual(request.AllergyInfo, product.AllergyInfo);
                Assert.AreEqual(request.StoreAdvertisedVegan, product.StoreAdvertisedVegan);
                Assert.AreEqual(request.LastScrapeDate, product.LastScrapeDate);
                Assert.AreEqual(request.ProductCategory.Name, product.ProductCategories.First().Name);
                Assert.IsFalse(product.IsProcessed);
            }
        }

        [TestMethod]
        public void Update_Valid()
        {
            // Arrange
            var request = new ProductUpdateRequest
            {
                Id = 100,
                IsProcessed = true
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.Update(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(request.IsProcessed, context.Products.Find(100).IsProcessed);
            }
        }

        [TestMethod]
        public void Delete_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.Delete(100);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(DefaultCount - 1, context.Products.Count());
                Assert.IsNotNull(context.Products.Find(101));
            }
        }
    }
}
