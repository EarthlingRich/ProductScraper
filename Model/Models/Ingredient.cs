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
        private string _keyWords { get; set; }
        [NotMapped]
        public string[] KeyWords
        {
            get { return _keyWords.Split(";"); }
            set { _keyWords = string.Join(";", value); }
        }

        public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
        {
            public void Configure(EntityTypeBuilder<Ingredient> builder)
            {
                builder.Property(_ => _._keyWords);
            }
        }
    }
}
