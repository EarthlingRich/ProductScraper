using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    public class Product
    {
        public Product() {
            MatchedIngredients = new JoinCollectionFacade<Ingredient, ProductIngredient>(
                ProductIngredients,
                _ => _.Ingredient,
                ingredient => new ProductIngredient { Product = this, Ingredient = ingredient }
            );
            ProductCategories = new JoinCollectionFacade<ProductCategory, ProductProductCategory>(
                ProductProductCategories,
                _ => _.ProductCategory,
                productCategory => new ProductProductCategory { Product = this, ProductCategory = productCategory }
            );
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public StoreType StoreType { get; set; }
        public string Url { get; set; }
        public string Ingredients { get; set; }
        public string AllergyInfo { get; set; }
        public VeganType VeganType { get; set; }
        public bool StoreAdvertisedVegan { get; set; }
        public bool IsProcessed { get; set; }
        public ProductCategory Category { get; set; }
        public DateTime LastScrapeDate { get; set; }

        public ICollection<ProductIngredient> ProductIngredients { get; } = new List<ProductIngredient>();
        [NotMapped]
        public ICollection<Ingredient> MatchedIngredients { get; }

        public ICollection<ProductProductCategory> ProductProductCategories { get; } = new List<ProductProductCategory>();
        [NotMapped]
        public ICollection<ProductCategory> ProductCategories { get; }

        public ICollection<WorkloadItem> WorkloadItems { get; set; }
        public ICollection<ProductActivity> ProductActivities { get; set; }
    }

    public class ProductIngredient
    {
        public int ProductId { get; set; }
        public int IngredientId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Ingredient Ingredient { get; set; }
    }

    public class ProductProductCategory
    {
        public int ProductId { get; set; }
        public int ProductCategoryId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
