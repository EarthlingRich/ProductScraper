using System;
using System.Collections.Generic;
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
        readonly DateTime ScrapeDate = DateTime.Now;

        public void AddTestData(ApplicationContext context)
        {
            var productCategory1 = new ProductCategory
            {
                Id = 100,
                Name = "Product category 1"
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
                Url = "Product 1 Url",
                StoreType = StoreType.AlbertHeijn,
                LastScrapeDate = ScrapeDate
            };
            product1.WorkloadItems.Add(new WorkloadItem { Id = 100 });
            product1.ProductActivities.Add(new ProductActivity { Id = 100 });
            product1.ProductCategories.Add(productCategory1);
            context.Products.Add(product1);

            var product2 = new Product
            {
                Id = 101,
                Name = "Product 2",
                Url = "Product 2 Url",
                StoreType = StoreType.Jumbo,
                LastScrapeDate = ScrapeDate
            };
            product2.WorkloadItems.Add(new WorkloadItem { Id = 101 });
            product2.ProductActivities.Add(new ProductActivity { Id = 101 });
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
                var product = context.Products.Include(p => p.WorkloadItems).Include("ProductProductCategories.ProductCategory").Single(p => p.Name == request.Name);
                Assert.AreEqual(DefaultCount + 1, context.Products.Count());
                Assert.AreEqual(request.Name, product.Name);
                Assert.AreEqual(request.StoreType, product.StoreType);
                Assert.AreEqual(request.Url, product.Url);
                Assert.AreEqual(request.Ingredients, product.Ingredients);
                Assert.AreEqual(request.AllergyInfo, product.AllergyInfo);
                Assert.AreEqual(request.StoreAdvertisedVegan, product.StoreAdvertisedVegan);
                Assert.AreEqual(request.LastScrapeDate, product.LastScrapeDate);
                Assert.AreEqual(request.ProductCategory.Name, product.ProductCategories.First().Name);
                Assert.AreEqual(1, product.WorkloadItems.Count(p => p.Message == "Nieuw product gevonden"));
                Assert.AreEqual(1, product.WorkloadItems.Count(p => p.Message == "Product is wel vegan volgens winkel"));
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
                    ProductCategory = context.ProductCategories.Find(101)
                };

                // Act
                var productService = new ProductApplicationService(context, _mapper);
                productService.CreateOrUpdate(request);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Include("ProductProductCategories.ProductCategory").Single(p => p.Id == 100);
                Assert.AreEqual(DefaultCount, context.Products.Count());
                Assert.AreEqual(request.Name, product.Name);
                Assert.AreEqual(request.StoreType, product.StoreType);
                Assert.AreEqual(request.Url, product.Url);
                Assert.AreEqual(request.Ingredients, product.Ingredients);
                Assert.AreEqual(request.AllergyInfo, product.AllergyInfo);
                Assert.AreEqual(request.StoreAdvertisedVegan, product.StoreAdvertisedVegan);
                Assert.AreEqual(request.LastScrapeDate, product.LastScrapeDate);
                Assert.AreEqual(1, product.ProductCategories.Count(p => p.Name == request.ProductCategory.Name));
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
                VeganType = VeganType.Vegan,
                IsProcessed = true,
                WorkloadItems = new List<ProductWorkloadItemRequest>
                {
                    new ProductWorkloadItemRequest
                    {
                        Id = 100,
                        IsProcessed = true
                    }
                }
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
                var product = context.Products.Include(p => p.WorkloadItems).Single(p => p.Id == 100);
                Assert.AreEqual(request.VeganType, product.VeganType);
                Assert.AreEqual(request.IsProcessed, product.IsProcessed);
                Assert.AreEqual(request.WorkloadItems.First().IsProcessed, product.WorkloadItems.First().IsProcessed);
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

        #region ProcessAllNonVegan

        [TestMethod]
        public void ProcessAllNonVegan_Not_Vegan_Valid()
        {
            // Arrange
            var product = new Product
            {
                Id = 200,
                Name = "Product 1",
                AllergyInfo = "test, notvegan, test"
            };

            var ingredient = new Ingredient
            {
                Id = 200,
                Name = "Ingredient 1",
                VeganType = VeganType.Not,
                AllergyKeywords = new[] { "notvegan" }
            };

            using (var context = new ApplicationContext(_options))
            {
                product.WorkloadItems.Add(new WorkloadItem());
                context.Products.Add(product);
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessAllNonVegan();
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var assertProduct = context.Products
                        .Include(p => p.WorkloadItems)
                        .Include(p => p.ProductActivities)
                        .Single(p => p.Id == 200);
                Assert.AreEqual(VeganType.Not, assertProduct.VeganType);
                Assert.IsTrue(assertProduct.IsProcessed);
                Assert.IsTrue(assertProduct.WorkloadItems.First().IsProcessed);

                Assert.AreEqual(2, assertProduct.ProductActivities.Count());
                var assertProductActivityIngredientAdded = assertProduct.ProductActivities.Single(pa => pa.Type == ProductActivityType.IngredientAdded);
                Assert.AreEqual(ingredient.Name, assertProductActivityIngredientAdded.Detail);
                var assertProductActivityVeganTypeChanged = assertProduct.ProductActivities.Single(pa => pa.Type == ProductActivityType.VeganTypeChanged);
                Assert.AreEqual(VeganType.Not.ToString(), assertProductActivityVeganTypeChanged.Detail);
            }
        }

        [TestMethod]
        public void ProcessAllNonVegan_Unsure_Valid()
        {
            // Arrange
            var product = new Product
            {
                Id = 200,
                Name = "Product 1",
                Ingredients = "test, notvegan, test"
            };

            var ingredient = new Ingredient
            {
                Id = 200,
                Name = "Ingredient 1",
                VeganType = VeganType.Unsure,
                KeyWords = new[] { "notvegan" }
            };

            using (var context = new ApplicationContext(_options))
            {
                product.WorkloadItems.Add(new WorkloadItem());
                context.Products.Add(product);
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessAllNonVegan();
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var assertProduct = context.Products
                        .Include(p => p.WorkloadItems)
                        .Include(p => p.ProductActivities)
                        .Single(p => p.Id == 200);
                Assert.AreEqual(VeganType.Unsure, assertProduct.VeganType);
                Assert.IsTrue(assertProduct.IsProcessed);
                Assert.IsTrue(assertProduct.WorkloadItems.First().IsProcessed);

                Assert.AreEqual(2, assertProduct.ProductActivities.Count());
                var assertProductActivityIngredientAdded = assertProduct.ProductActivities.Single(pa => pa.Type == ProductActivityType.IngredientAdded);
                Assert.AreEqual(ingredient.Name, assertProductActivityIngredientAdded.Detail);
                var assertProductActivityVeganTypeChanged = assertProduct.ProductActivities.Single(pa => pa.Type == ProductActivityType.VeganTypeChanged);
                Assert.AreEqual(VeganType.Unsure.ToString(), assertProductActivityVeganTypeChanged.Detail);
            }
        }

        [TestMethod]
        public void ProcessAllNonVegan_Unsure_NeedsReview_Valid()
        {
            // Arrange
            var product = new Product
            {
                Id = 200,
                Name = "Product 1",
                Ingredients = "test, notvegan, test"
            };

            var ingredient = new Ingredient
            {
                Id = 200,
                Name = "Ingredient 1",
                VeganType = VeganType.Unsure,
                NeedsReview = true,
                KeyWords = new[] { "notvegan" }
            };
            using (var context = new ApplicationContext(_options))
            {
                product.WorkloadItems.Add(new WorkloadItem());
                context.Products.Add(product);
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessAllNonVegan();
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var assertProduct = context.Products.Include(p => p.WorkloadItems).Single(p => p.Id == 200);
                Assert.AreEqual(VeganType.Unsure, assertProduct.VeganType);
                Assert.IsFalse(assertProduct.IsProcessed);
                Assert.IsFalse(assertProduct.WorkloadItems.First().IsProcessed);
            }
        }

        [TestMethod]
        public void ProcessAllNonVegan_No_Match_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    Ingredients = "test, test",
                    AllergyInfo = "test, test",
                    VeganType = VeganType.Unkown
                };
                product.WorkloadItems.Add(new WorkloadItem());
                context.Products.Add(product);

                var ingredient = new Ingredient
                {
                    Id = 200,
                    Name = "Ingredient 1",
                    VeganType = VeganType.Not,
                    KeyWords = new[] { "notvegan" },
                    AllergyKeywords = new[] { "notvegan" }
                };
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessAllNonVegan();
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var assertProduct = context.Products
                        .Include(p => p.WorkloadItems)
                        .Include(p => p.ProductActivities)
                        .Single(p => p.Id == 200);
                Assert.AreEqual(VeganType.Unkown, assertProduct.VeganType);
                Assert.IsFalse(assertProduct.IsProcessed);
                Assert.AreEqual(1, assertProduct.WorkloadItems.Count());
                Assert.IsFalse(assertProduct.WorkloadItems.First().IsProcessed);
                Assert.AreEqual(0, assertProduct.ProductActivities.Count());
            }
        }

        [TestMethod]
        public void ProcessAllNonVegan_StoreAdvertisedVegan_Ignore_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    VeganType = VeganType.Unkown,
                    StoreAdvertisedVegan = true
                };
                product.WorkloadItems.Add(new WorkloadItem());
                context.Products.Add(product);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessAllNonVegan();
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var assertProduct = context.Products.Include(p => p.WorkloadItems).Single(p => p.Id == 200);
                Assert.AreEqual(VeganType.Unkown, assertProduct.VeganType);
                Assert.IsFalse(assertProduct.IsProcessed);
                Assert.IsFalse(assertProduct.WorkloadItems.First().IsProcessed);
            }
        }

        #endregion

        [TestMethod]
        public void ProcessVeganType_Not_Vegan_Ingredient_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    Ingredients = "test, notvegan, test"
                };
                context.Products.Add(product);

                var ingredient = new Ingredient
                {
                    Id = 200,
                    Name = "Ingredient 1",
                    VeganType = VeganType.Not,
                    KeyWords = new[] { "notvegan" }
                };
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessVeganType(200);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Find(200);
                Assert.AreEqual(VeganType.Not, product.VeganType);
            }
        }

        [TestMethod]
        public void ProcessVeganType_Not_Vegan_AllergyInfo_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    AllergyInfo = "test, notvegan , test"
                };
                context.Products.Add(product);

                var ingredient = new Ingredient
                {
                    Id = 200,
                    Name = "Ingredient 1",
                    VeganType = VeganType.Not,
                    AllergyKeywords = new[] { "notvegan" }
                };
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessVeganType(200);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Find(200);
                Assert.AreEqual(VeganType.Not, product.VeganType);
            }
        }

        [TestMethod]
        public void ProcessVeganType_Unsure_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    Ingredients = "test, notvegan, test",
                    AllergyInfo = "test, notvegan , test"
                };
                context.Products.Add(product);

                var ingredient = new Ingredient
                {
                    Id = 200,
                    Name = "Ingredient 1",
                    VeganType = VeganType.Unsure,
                    KeyWords = new[] { "notvegan" },
                    AllergyKeywords= new[] { "notvegan" }
                };
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessVeganType(200);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Find(200);
                Assert.AreEqual(VeganType.Unsure, product.VeganType);
            }
        }

        [TestMethod]
        public void ProcessVeganType_Vegan_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    Ingredients = "test, test",
                    AllergyInfo = "test, test"
                };
                context.Products.Add(product);

                var ingredient = new Ingredient
                {
                    Id = 200,
                    Name = "Ingredient 1",
                    VeganType = VeganType.Not,
                    KeyWords = new[] { "notvegan" },
                    AllergyKeywords = new[] { "notvegan" }
                };
                context.Ingredients.Add(ingredient);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessVeganType(200);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Find(200);
                Assert.AreEqual(VeganType.Vegan, product.VeganType);
            }
        }

        [TestMethod]
        public void ProcessVeganType_StoreAdvertisedVegan_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                var product = new Product
                {
                    Id = 200,
                    Name = "Product 1",
                    StoreAdvertisedVegan = true
                };
                context.Products.Add(product);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.ProcessVeganType(200);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var product = context.Products.Find(200);
                Assert.AreEqual(VeganType.Vegan, product.VeganType);
            }
        }

        [TestMethod]
        public void RemoveOutdatedProducts_Valid()
        {
            // Arrange
            using (var context = new ApplicationContext(_options))
            {
                AddTestData(context);

                var product3 = new Product
                {
                    Id = 200,
                    Name = "Product 3",
                    StoreType = StoreType.AlbertHeijn,
                    LastScrapeDate = ScrapeDate.AddDays(-1)
                };
                context.Products.Add(product3);

                var product4 = new Product
                {
                    Id = 201,
                    Name = "Product 4",
                    StoreType = StoreType.Jumbo,
                    LastScrapeDate = ScrapeDate.AddDays(-1)
                };
                context.Products.Add(product4);

                context.SaveChanges();

                var productService = new ProductApplicationService(context, _mapper);

                // Act
                productService.RemoveOutdatedProducts(StoreType.AlbertHeijn, ScrapeDate);
            }

            //Assert
            using (var context = new ApplicationContext(_options))
            {
                var productsNotOudated = context.Products.Include(p => p.WorkloadItems).Where(p => p.Id != 200);
                foreach (var productNotOutdated in productsNotOudated)
                {
                    Assert.AreEqual(0, productNotOutdated.WorkloadItems.Count(w => w.Message == "Product niet gevonden"));
                }

                var productOudated = context.Products.Include(p => p.WorkloadItems).Single(p => p.Id == 200);
                Assert.AreEqual(1, productOudated.WorkloadItems.Count());
                Assert.AreEqual("Product niet gevonden", productOudated.WorkloadItems.First().Message);
            }
        }
    }
}
