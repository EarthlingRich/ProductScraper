using System.ComponentModel.DataAnnotations;
using Model.Resources;

namespace Api.Models
{
    public class WorkloadProductListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public string VeganType { get; set; }

        [Display(Name = "Product_IsNew", ResourceType = typeof(DomainTerms))]
        public bool IsNew { get; set; }
    }
}
