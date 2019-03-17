using System.ComponentModel.DataAnnotations;
using Model.Models;
using Model.Resources;

namespace Api.Models
{
    public class WorkloadItemListViewModel
    {
        public int ProductId { get; set; }

        [Display(Name = "Product_Name", ResourceType = typeof(DomainTerms))]
        public string ProductName { get; set; }

        [Display(Name = "Product_StoreType", ResourceType = typeof(DomainTerms))]
        public StoreType StoreType { get; set; }

        [Display(Name = "WorkloadItem_Message", ResourceType = typeof(DomainTerms))]
        public string Message { get; set; }

        [Display(Name = "WorkloadItem_CreatedOn", ResourceType = typeof(DomainTerms))]
        public string CreatedOn { get; set; }
    }
}
