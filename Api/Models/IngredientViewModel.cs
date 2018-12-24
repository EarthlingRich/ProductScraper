using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Model.Models;
using Model.Resources;

namespace Api.Models
{
    public class IngredientCreateViewModel
    {
        public IngredientCreateRequest Request { get; set; }

        public IngredientCreateViewModel()
        {
            Request = new IngredientCreateRequest();
        }
    }

    public class IngredientCreateRequest
    {
        [Display(Name = "Ingredient_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }
    }

    public class IngredientUpdateViewModel
    {
        public IngredientUpdateRequest Request { get; set; }

        public static IngredientUpdateViewModel Map(Ingredient ingredient, IMapper mapper)
        {
            return new IngredientUpdateViewModel
            {
                Request = mapper.Map<IngredientUpdateRequest>(ingredient)
            };
        }
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

    public class IngredientListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Ingredient_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        [Display(Name = "Ingredient_VeganType", ResourceType = typeof(DomainTerms))]
        public string VeganType { get; set; }
    }
}
