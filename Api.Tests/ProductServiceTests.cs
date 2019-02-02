using System;
using System.Linq;
using Api.Models;
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
    public class ProductServiceTests
    {
        DbContextOptions<ApplicationContext> _options;
        IMapper _mapper;

        public void AddTestData(ApplicationContext context)
        {
            context.Add(new Product
            {
                Id = 1,
                Name = "Product 1",
            });
            context.Add(new Product
            {
                Id = 2,
                Name = "Product 2",
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
        public void Update_Valid()
        {
            // Arrange
            var request = new ProductUpdateRequest
            {
                Id = 1,
                IsProcessed = true
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productService = new ProductService(context, _mapper);

                // Act
                productService.Update(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(request.IsProcessed, context.Products.Find(1).IsProcessed);
            }
        }

        [TestMethod]
        public void Delete_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var productService = new ProductService(context, _mapper);

                // Act
                productService.Delete(1);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(1, context.Products.Count());
                //Assert.IsNotNull(context.Products.Find(2));
            }
        }
    }
}
