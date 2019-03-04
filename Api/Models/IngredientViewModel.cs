using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Model.Models;
using Model.Requests;
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

    public class IngredientUpdateViewModel
    {
        public IngredientUpdateRequest Request { get; set; }
        public List<string> AllergyKeywords { get; set; }
        public List<string> IgnoreKeywords { get; set; }
        public List<string> Keywords { get; set; }

        public IngredientUpdateViewModel()
        {
            AllergyKeywords = new List<string>();
            IgnoreKeywords = new List<string>();
            Keywords = new List<string>();
        }

        public static IngredientUpdateViewModel Map(Ingredient ingredient, IMapper mapper)
        {
            var ingredientUpdateViewModel = new IngredientUpdateViewModel
            {
                Request = mapper.Map<IngredientUpdateRequest>(ingredient)
            };

            ingredientUpdateViewModel.AllergyKeywords.AddRange(ingredient.AllergyKeywords);
            ingredientUpdateViewModel.IgnoreKeywords.AddRange(ingredient.IgnoreKeywords);
            ingredientUpdateViewModel.Keywords.AddRange(ingredient.Keywords);

            return ingredientUpdateViewModel;
        }
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
