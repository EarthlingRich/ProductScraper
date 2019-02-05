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
        public List<string> Keywords { get; set; }
        public List<string> AllergyKeywords { get; set; }

        public IngredientUpdateViewModel()
        {
            Keywords = new List<string>();
            AllergyKeywords = new List<string>();
        }

        public static IngredientUpdateViewModel Map(Ingredient ingredient, IMapper mapper)
        {
            var ingredientUpdateViewModel = new IngredientUpdateViewModel
            {
                Request = mapper.Map<IngredientUpdateRequest>(ingredient)
            };

            ingredientUpdateViewModel.Keywords.AddRange(ingredient.KeyWords);
            ingredientUpdateViewModel.AllergyKeywords.AddRange(ingredient.AllergyKeywords);

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
