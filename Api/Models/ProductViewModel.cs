using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Model.Models;
using Model.Requests;
using Model.Resources;

namespace Api.Models
{
    public class ProductUpdateViewModel
    {
        public ProductUpdateViewModel()
        {
            MatchedIngredients = new List<Ingredient>();
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "Product_Ingredients", ResourceType = typeof(DomainTerms))]
        public string Ingredients { get; set; }

        [Display(Name = "Product_AllergyInfo", ResourceType = typeof(DomainTerms))]
        public string AllergyInfo { get; set; }

        [Display(Name = "Product_MatchedIngredients", ResourceType = typeof(DomainTerms))]
        public List<Ingredient> MatchedIngredients { get; set; }

        [Display(Name = "Product_ProductCategories", ResourceType = typeof(DomainTerms))]
        public List<string> ProductCategories { get; set; }

        [Display(Name = "Product_ProductActivities", ResourceType = typeof(DomainTerms))]
        public List<ProductActivity> ProductActivities { get; set; }

        [Display(Name = "Product_WorkloadItems", ResourceType = typeof(DomainTerms))]
        public List<ProductUpdateWorkloadItemViewModel> WorkloadItems { get; set; }

        public ProductUpdateRequest Request { get; set; }

        public static ProductUpdateViewModel Map(Product product, IMapper mapper)
        {
            var viewModel = mapper.Map<ProductUpdateViewModel>(product);
            viewModel.Request = mapper.Map<ProductUpdateRequest>(product);
            viewModel.WorkloadItems = mapper.Map<List<ProductUpdateWorkloadItemViewModel>>(product.WorkloadItems);

            return viewModel;
        }
    }

    public class ProductUpdateWorkloadItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "WorkloadItem_Message", ResourceType = typeof(DomainTerms))]
        public string Message { get; set; }

        [Display(Name = "WorkloadItem_CreatedOn", ResourceType = typeof(DomainTerms))]
        public DateTime CreatedOn { get; set; }
    }

    public class ProductListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        [Display(Name = "Product_StoreType", ResourceType = typeof(DomainTerms))]
        public StoreType StoreType { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public string VeganType { get; set; }

        [Display(Name = "Product_ProductCategories", ResourceType = typeof(DomainTerms))]
        public string ProductCategories { get; set; } 
    }

    public class ProductActivityListViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string ProductName { get; set; }

        [Display(Name = "Product_StoreType", ResourceType = typeof(DomainTerms))]
        public StoreType ProductStoreType { get; set; }

        [Display(Name = "ProductActivity_ProductActivityType", ResourceType = typeof(DomainTerms))]
        public string Type { get; set; }

        [Display(Name = "ProductActivity_Detail", ResourceType = typeof(DomainTerms))]
        public string Detail { get; set; }

        [Display(Name = "ProductActivity_CreatedOn", ResourceType = typeof(DomainTerms))]
        public string CreatedOn { get; set; }
    }
}
