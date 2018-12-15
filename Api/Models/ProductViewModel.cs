using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Model.Models;
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

        public ProductUpdateRequest Request { get; set; }
    }

    public class ProductUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public int VeganType { get; set; }

        [Display(Name = "Product_IsProcessed", ResourceType = typeof(DomainTerms))]
        public bool IsProcessed { get; set; }

        [Display(Name = "Product_IsNew", ResourceType = typeof(DomainTerms))]
        public bool IsNew { get; set; }
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
