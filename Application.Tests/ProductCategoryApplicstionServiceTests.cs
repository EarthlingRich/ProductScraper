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
    public class ProductCategoryServiceApplicationTests
    {
        DbContextOptions<ApplicationContext> _options;
        IMapper _mapper;
        readonly int DefaultCount = 2;

        public void AddTestData(ApplicationContext context)
        {
            context.Add(new ProductCategory
            {
                Id = 100,
                Name = "Product Category 1",
            });
            context.Add(new ProductCategory
            {
                Id = 101,
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

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new ApplicationMapperConfiguration())
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
                AddTestData(context);
                var productCategoryService = new ProductCategoryApplicationService(context, _mapper);

                // Act
                productCategoryService.Create(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(DefaultCount + 1, context.ProductCategories.Count());
                Assert.AreEqual(request.Name, context.ProductCategories.Find(1).Name);
            }
        }

        [TestMethod]
        public void Update_Valid()
        {
            // Arrange
            var request = new ProductCategoryUpdateRequest
            {
                Id = 100,
                Name = "Product Category Update"
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productCategoryService = new ProductCategoryApplicationService(context, _mapper);

                // Act
                productCategoryService.Update(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(request.Name, context.ProductCategories.Find(100).Name);
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
                var productCategoryService = new ProductCategoryApplicationService(context, _mapper);

                // Act
                result = productCategoryService.Delete(100);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsTrue(result);
                Assert.AreEqual(DefaultCount - 1, context.ProductCategories.Count());
                Assert.IsNull(context.ProductCategories.Find(100));
                Assert.IsNotNull(context.ProductCategories.Find(101));
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
                product.ProductCategories.Add(context.ProductCategories.Find(100));
                context.Products.Add(product);
                context.SaveChanges();

                var productCategoryService = new ProductCategoryApplicationService(context, _mapper);

                // Act
                result = productCategoryService.Delete(100);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsFalse(result);
                Assert.AreEqual(DefaultCount, context.ProductCategories.Count());
            }
        }
    }
}
