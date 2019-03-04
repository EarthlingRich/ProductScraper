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
    public class IngredientApplicationServiceTests
    {
        DbContextOptions<ApplicationContext> _options;
        IMapper _mapper;
        readonly int DefaultCount = 2;

        public void AddTestData(ApplicationContext context)
        {
            context.Add(new Ingredient
            {
                Id = 100,
                Name = "Ingredient 1",
            });
            context.Add(new Ingredient
            {
                Id = 101,
                Name = "Ingredient 2",
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
            var request = new IngredientCreateRequest
            {
                Name = "Ingredient Create"
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var ingredientService = new IngredientApplicationService(context, _mapper);

                // Act
                ingredientService.Create(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.AreEqual(DefaultCount + 1, context.Ingredients.Count());
                Assert.AreEqual(request.Name, context.Ingredients.Find(1).Name);
            }
        }

        [TestMethod]
        public void Update_Valid()
        {
            // Arrange
            var request = new IngredientUpdateRequest
            {
                Id = 100,
                Name = "Ingredient Update",
                KeywordsString = "keyword1;keyword2",
                AllergyKeywordsString = "allergy1;allergy2"
            };

            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);
                var ingredientService = new IngredientApplicationService(context, _mapper);

                // Act
                ingredientService.Update(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var ingredient = context.Ingredients.Find(100);
                Assert.AreEqual(request.Name, ingredient.Name);
                Assert.AreEqual(request.KeywordsString, ingredient.KeywordsString);
                CollectionAssert.AreEqual(new string[] { "keyword1", "keyword2" }, ingredient.Keywords);
                Assert.AreEqual(request.AllergyKeywordsString, ingredient.AllergyKeywordsString);
                CollectionAssert.AreEqual(new string[] { "allergy1", "allergy2" }, ingredient.AllergyKeywords);
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
                var ingredientService = new IngredientApplicationService(context, _mapper);

                // Act
                result = ingredientService.Delete(100);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsTrue(result);
                Assert.AreEqual(DefaultCount - 1, context.Ingredients.Count());
                Assert.IsNull(context.Ingredients.Find(100));
                Assert.IsNotNull(context.Ingredients.Find(101));
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
                product.MatchedIngredients.Add(context.Ingredients.Find(100));
                context.Products.Add(product);
                context.SaveChanges();

                var ingredientService = new IngredientApplicationService(context, _mapper);

                // Act
                result = ingredientService.Delete(100);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                Assert.IsFalse(result);
                Assert.AreEqual(2, context.Ingredients.Count());
            }
        }
    }
}
