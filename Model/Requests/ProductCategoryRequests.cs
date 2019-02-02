using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Model.Models;
using Model.Resources;

namespace Model.Requests
{
    public class ProductCategoryCreateRequest
    {
        [Display(Name = "ProductCategory_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }
    }

    public class ProductCategoryUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "ProductCategory_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        public List<StoreCategory> StoreCategories { get; set; }
    }
}
