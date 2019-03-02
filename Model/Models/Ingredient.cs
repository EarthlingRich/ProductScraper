using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Model.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool NeedsReview { get; set; }
        public VeganType VeganType { get; set; }

        [Column("Keywords")]
        public string KeywordsString { get; set; }
        [NotMapped]
        public string[] Keywords
        {
            get { return !string.IsNullOrEmpty(KeywordsString) ? KeywordsString.Split(";") : new string[0]; }
            set { KeywordsString = string.Join(";", value).ToLower(); }
        }

        [Column("IgnoreKeywords")]
        public string IgnoreKeywordsString { get; set; }
        [NotMapped]
        public string[] IgnoreKeywords
        {
            get { return !string.IsNullOrEmpty(IgnoreKeywordsString) ? IgnoreKeywordsString.Split(";") : new string[0]; }
            set { IgnoreKeywordsString = string.Join(";", value).ToLower(); }
        }

        [Column("AllergyKeywords")]
        public string AllergyKeywordsString { get; set; }
        [NotMapped]
        public string[] AllergyKeywords
        {
            get { return !string.IsNullOrEmpty(AllergyKeywordsString) ? AllergyKeywordsString.Split(";") : new string[0]; }
            set { AllergyKeywordsString = string.Join(";", value).ToLower(); }
        }

        public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
        {
            public void Configure(EntityTypeBuilder<Ingredient> builder)
            {
                builder.Property(_ => _.KeywordsString);
                builder.Property(_ => _.AllergyKeywordsString);
            }
        }
    }
}
