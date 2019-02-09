using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Model.Models;
using Model.Resources;

namespace Model.Requests
{
    public class ProductStoreRequest
    {
        public string Name { get; set; }
        public StoreType StoreType { get; set; }
        public string Url { get; set; }
        public string AllergyInfo { get; set; }
        public bool StoreAdvertisedVegan { get; set; }
        public string Ingredients { get; set; }
        public DateTime LastScrapeDate { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }

    public class ProductUpdateRequest
    {
        public int Id { get; set; }

        [Display(Name = "Product_VeganType", ResourceType = typeof(DomainTerms))]
        public VeganType VeganType { get; set; }

        [Display(Name = "Product_IsProcessed", ResourceType = typeof(DomainTerms))]
        public bool IsProcessed { get; set; }

        public List<ProductWorkloadItemRequest> WorkloadItems { get; set; }
    }

    public class ProductWorkloadItemRequest
    {
        public int Id { get; set; }

        [Display(Name = "WorkloadItem_IsProcessed", ResourceType = typeof(DomainTerms))]
        public bool IsProcessed { get; set; }
    }
}
