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
                .UseInMemoryDatabase(databaseName: "ProductScraper" + DateTime.Now)
                .Options;
                
            var config = new AutoMapper.MapperConfiguration(cfg =>
                cfg.AddProfile(new Models.MapperConfiguration())
            );
            _mapper = new Mapper(config);
        }

        [TestMethod]
        public void Update_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var proudctService = new ProductService(context, _mapper);

                // Act
                proudctService.Update(new ProductUpdateRequest
                {
                    Id = 1,
                    IsProcessed = true
                });
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsTrue(context.Products.Find(1).IsProcessed);
            }
        }

        [TestMethod]
        public void Delete_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var proudctService = new ProductService(context, _mapper);

                // Act
                proudctService.Delete(1);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(1, context.Products.Count());
                Assert.AreEqual("Product 2", context.Products.Find(2).Name);
            }
        }
    }
}
