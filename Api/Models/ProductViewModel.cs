using System.Collections.Generic;
using Model.Models;

namespace Api.Models
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            MatchedIngredients = new List<Ingredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Ingredients { get; set; }
        public string AllergyInfo { get; set; }
        public List<Ingredient> MatchedIngredients { get; set; }
        public VeganType VeganType { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsNew { get; set; }
    }

    public class ProductApiViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Ingredients { get; set; }
        public VeganType VeganType { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsNew { get; set; }
    }
}
