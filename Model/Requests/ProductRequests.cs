using System.ComponentModel.DataAnnotations;
using Model.Resources;

namespace Model.Requests
{
    public class ProductUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public int VeganType { get; set; }

        [Display(Name = "Product_IsProcessed", ResourceType = typeof(DomainTerms))]
        public bool IsProcessed { get; set; }

        [Display(Name = "Product_IsNew", ResourceType = typeof(DomainTerms))]
        public bool IsNew { get; set; }

    }
}
