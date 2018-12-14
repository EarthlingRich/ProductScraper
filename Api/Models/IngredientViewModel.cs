using Model.Models;

namespace Api.Models
{
    public class IngredientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string KeywordsString { get; set; }
        public string[] Keywords { get; set; }
        public string AllergyKeywordsString { get; set; }
        public string[] AllergyKeywords { get; set; }
        public VeganType VeganType { get; set; }
    }

    public class IngredientListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VeganType { get; set; }
    }
}
