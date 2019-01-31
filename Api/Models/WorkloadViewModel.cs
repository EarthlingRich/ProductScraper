using System.ComponentModel.DataAnnotations;
using Model.Resources;

namespace Api.Models
{
    public class WorkloadItemListViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string ProductName { get; set; }

        [Display(Name = "WorkLoadItem_Message", ResourceType = typeof(DomainTerms))]
        public string Message { get; set; }

        [Display(Name = "WorkLoadItem_CreatedOn", ResourceType = typeof(DomainTerms))]
        public string CreatedOn { get; set; }
    }
}
