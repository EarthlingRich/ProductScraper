using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Model.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("Keywords")]
        public string KeywordsString { get; set; }
        [NotMapped]
        public string[] KeyWords
        {
            get { return KeywordsString != null ? KeywordsString.Split(";") : new string[0]; }
            set { KeywordsString = string.Join(";", value); }
        }
        [Column("AllergyKeyWords")]
        public string AllergyKeywordsString { get; set; }
        [NotMapped]
        public string[] AllergyKeywords
        {
            get { return AllergyKeywordsString != null ? AllergyKeywordsString.Split(";") : new string[0]; }
            set { AllergyKeywordsString = string.Join(";", value); }
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
