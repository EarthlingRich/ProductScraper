using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Models
{
    [Table("StoreCategories")]
    public class StoreCategory
    {
        public int Id { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public StoreType StoreType { get; set; }
        public string Url { get; set; }
    }
}
