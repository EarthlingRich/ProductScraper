using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Model.Models;
using Model.Requests;
using Model.Resources;

namespace Api.Models
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "ProductCategory_Name", ResourceType = typeof(DomainTerms))]
        public string Name { get; set; }

        public ProductCategoryViewModel Map(ProductCategory productCategory, IMapper mapper)
        {
            return mapper.Map<ProductCategoryViewModel>(productCategory);
        }
    }

    public class ProductCategoryCreateViewModel
    {
        public ProductCategoryCreateRequest Request { get; set; }

        public ProductCategoryCreateViewModel()
        {
            Request = new ProductCategoryCreateRequest();
        }
    }

    public class ProductCategoryUpdateViewModel
    {
        public ProductCategoryUpdateRequest Request { get; set; }

        public static ProductCategoryUpdateViewModel Map(ProductCategory productCategory, IMapper mapper)
        {
            return new ProductCategoryUpdateViewModel
            {
                Request = mapper.Map<ProductCategoryUpdateRequest>(productCategory)
            };
        }
    }
}
