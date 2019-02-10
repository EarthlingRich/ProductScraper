using System;

namespace Model.Models
{
    public class ProductActivity
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public ProductActivityType Type { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
