using System;
using System.Linq;
using Api.Models;
using Api.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Models;

namespace Api.Tests
{
    [TestClass]
    public class ProductCategoryServiceTests
    {
        DbContextOptions<ApplicationContext> _options;
        IMapper _mapper;

        public void AddTestData(ApplicationContext context)
        {
            context.Add(new ProductCategory
            {
                Id = 1,
                Name = "Product Category 1",
            });
            context.Add(new ProductCategory
            {
                Id = 2,
                Name = "Product Category 2",
            });
            context.SaveChanges();
        }

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "ProductScraper" + Guid.NewGuid())
                .Options;

            var config = new AutoMapper.MapperConfiguration(cfg =>
                cfg.AddProfile(new Models.MapperConfiguration())
            );
            _mapper = new Mapper(config);
        }

        [TestMethod]
        public void Create_Valid()
        {
            // Arrange
            var request = new ProductCategoryCreateRequest
            {
                Name = "Product Category Create"
            };

            using (var context = new ApplicationContext(_options))
            {
                var productCategoryService = new ProductCategoryService(context, _mapper);

                // Act
                productCategoryService.Create(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(1, context.ProductCategories.Count());
                Assert.AreEqual(request.Name, context.ProductCategories.First().Name);
            }
        }

        [TestMethod]
        public void Update_Valid()
        {
            // Arrange
            var request = new ProductCategoryUpdateRequest
            {
                Id = 1,
                Name = "Product Category Update"
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productCategoryService = new ProductCategoryService(context, _mapper);

                // Act
                productCategoryService.Update(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(request.Name, context.ProductCategories.Find(1).Name);
            }
        }

        [TestMethod]
        public void Delete_Valid()
        {
            // Arrange
            bool result;

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productCategoryService = new ProductCategoryService(context, _mapper);

                // Act
                result = productCategoryService.Delete(1);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.ProductCategories.Count());
                Assert.IsNull(context.ProductCategories.Find(1));
            }
        }

        [TestMethod]
        public void Delete_InUse()
        {
            // Arrange
            bool result;

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);

                var product = new Product();
                product.ProductCategories.Add(context.ProductCategories.Find(1));
                context.Products.Add(product);
                context.SaveChanges();

                var productCategoryService = new ProductCategoryService(context, _mapper);

                // Act
                result = productCategoryService.Delete(1);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsFalse(result);
                Assert.AreEqual(2, context.ProductCategories.Count());
                Assert.IsNotNull(context.ProductCategories.Find(1));
            }
        }
    }
}
