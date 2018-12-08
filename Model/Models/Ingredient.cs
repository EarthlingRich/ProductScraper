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
        public string KeyWordsString { get; set; }
        [NotMapped]
        public string[] KeyWords
        {
            get { return KeyWordsString != null ? KeyWordsString.Split(";") : new string[0]; }
            set { KeyWordsString = string.Join(";", value); }
        }

        public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
        {
            public void Configure(EntityTypeBuilder<Ingredient> builder)
            {
                builder.Property(_ => _.KeyWordsString);
            }
        }
    }
}
