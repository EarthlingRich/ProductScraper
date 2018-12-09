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
        public List<Ingredient> MatchedIngredients { get; set; }
        public bool IsVegan { get; set; }
    }
}
