using System;
using Model.Models;

namespace Api.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Ingredients { get; set; }
    }
}
