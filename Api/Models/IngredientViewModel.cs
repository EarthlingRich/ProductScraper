using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class IngredientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] KeyWords { get; set; }
    }
}
