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

        [Display(Name = "Product_Ingredients", ResourceType = typeof(DomainTerms))]
        public string Ingredients { get; set; }

        [Display(Name = "Product_AllergyInfo", ResourceType = typeof(DomainTerms))]
        public string AllergyInfo { get; set; }

        [Display(Name = "Product_MatchedIngredients", ResourceType = typeof(DomainTerms))]
        public List<Ingredient> MatchedIngredients { get; set; }

        public List<WorkloadItem> WorkloadItems { get; set; }

        [Display(Name = "Product_ProductCategories", ResourceType = typeof(DomainTerms))]
        public List<string> ProductCategories { get; set; }

        public ProductUpdateRequest Request { get; set; }

        public static ProductUpdateViewModel Map(Product product, IMapper mapper)
        {
            var viewModel = mapper.Map<ProductUpdateViewModel>(product);
            viewModel.Request = mapper.Map<ProductUpdateRequest>(product);

            return viewModel;
        }
    }

    public class ProductListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public string VeganType { get; set; }

        [Display(Name = "Product_IsNew", ResourceType = typeof(DomainTerms))]
        public bool IsNew { get; set; }

        [Display(Name = "Product_ProductCategories", ResourceType = typeof(DomainTerms))]
        public string ProductCategories { get; set; } 
    }

    public class ProductApiViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Ingredients { get; set; }
        public int VeganType { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsNew { get; set; }
    }
}
