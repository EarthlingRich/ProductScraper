using System.ComponentModel.DataAnnotations;
using Model.Models;
using Model.Resources;

namespace Model.Requests
{
    public class IngredientCreateRequest
    {
        [Display(Name = "Ingredient_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }
    }

    public class IngredientUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "Ingredient_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        [Display(Name = "Ingredient_NeedsReview", ResourceType = typeof(DomainTerms))]
        public bool NeedsReview { get; set; }

        [Display(Name = "Ingredient_VeganType", ResourceType = typeof(DomainTerms))]
        public VeganType VeganType { get; set; }

        public string KeywordsString { get; set; }
        [Display(Name = "Ingredient_Keywords", ResourceType = typeof(DomainTerms))]
        public string[] Keywords { get; set; }

        public string AllergyKeywordsString { get; set; }
        [Display(Name = "Ingredient_AllergyKeywords", ResourceType = typeof(DomainTerms))]
        public string[] AllergyKeywords { get; set; }
    }
}
