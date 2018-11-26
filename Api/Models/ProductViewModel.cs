using System;
using Model.Models;

namespace Api.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ProductViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
        }
    }
}
