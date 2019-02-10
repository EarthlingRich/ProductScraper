using System.Collections.Generic;

namespace Model.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<StoreCategory> StoreCategories { get; set; }
    }
}
