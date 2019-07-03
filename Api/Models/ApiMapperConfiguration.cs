using System.Linq;
using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class ApiMapperConfiguration : Profile
    {
        public ApiMapperConfiguration()
        {
            //Ingredient
            CreateMap<Ingredient, IngredientListViewModel>(MemberList.Destination);
            CreateMap<Ingredient, IngredientUpdateViewModel>(MemberList.Destination)
                .ForMember(_ => _.Request, opt => opt.Ignore());
            CreateMap<Ingredient, IngredientCreateViewModel>(MemberList.Destination)
                .ForMember(_ => _.Request, opt => opt.Ignore());

            //Product
            CreateMap<Product, ProductListViewModel>(MemberList.Destination)
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => string.Join(", ", p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _))));
            CreateMap<Product, ProductUpdateViewModel>(MemberList.Destination)
                .ForMember(_ => _.Request, opt => opt.Ignore())
                .ForMember(_ => _.WorkloadItems, opt => opt.MapFrom(wi => wi.WorkloadItems.OrderByDescending(_ => _.CreatedOn)))
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _)));
            CreateMap<WorkloadItem, ProductUpdateWorkloadItemViewModel>(MemberList.Destination);

            //ProductActivity
            CreateMap<ProductActivity, ProductActivityListViewModel>(MemberList.Destination);

            //ProductCategory
            CreateMap<ProductCategory, ProductCategoryViewModel>(MemberList.Destination);
            CreateMap<ProductCategory, ProductCategoryUpdateViewModel>(MemberList.Destination)
                .ForMember(_ => _.Request, opt => opt.Ignore());
            CreateMap<ProductCategory, ProductCategoryCreateViewModel>(MemberList.Destination)
                .ForMember(_ => _.Request, opt => opt.Ignore());

            //WorkloadItem
            CreateMap<WorkloadItem, WorkloadItemListViewModel>(MemberList.Destination);
        }
    }
}
