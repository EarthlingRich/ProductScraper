using System;
using System.Linq;
using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class ApiMapperConfiguration : Profile
    {
        public ApiMapperConfiguration()
        {
            //Product
            CreateMap<Product, ProductListViewModel>()
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => string.Join(", ", p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _))));
            CreateMap<Product, ProductUpdateViewModel>()
                .ForMember(_ => _.Request, opt => opt.Ignore())
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _)));
                
            //WorkLoadItem
            CreateMap<WorkloadItem, WorkloadItemListViewModel>(MemberList.Source)
                .ForMember(_ => _.ProductId, opt => opt.MapFrom(_ => _.Product.Id))
                .ForMember(_ => _.ProductName, opt => opt.MapFrom(_ => _.Product.Name));
        }
    }
}
