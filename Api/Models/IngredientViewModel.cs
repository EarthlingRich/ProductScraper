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
    }
}
