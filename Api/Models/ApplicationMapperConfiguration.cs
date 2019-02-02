using System.Linq;
using AutoMapper;
using Model.Models;
using Model.Requests;

namespace Api.Models
{
    public class ApplicationMapperConfiguration : Profile
    {
        public ApplicationMapperConfiguration()
        {
            //Ingredient
            CreateMap<IngredientUpdateRequest, Ingredient>(MemberList.Source)
                .ForMember(_ => _.KeyWords, opt => opt.Ignore())
                .ForMember(_ => _.AllergyKeywords, opt => opt.Ignore());
            CreateMap<IngredientCreateRequest, Ingredient>(MemberList.Source);

            //Product
            CreateMap<Product, ProductListViewModel>()
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => string.Join(", ", p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _))));
            CreateMap<Product, ProductUpdateViewModel>()
                .ForMember(_ => _.Request, opt => opt.Ignore())
                .ForMember(_ => _.ProductCategories, opt => opt.MapFrom(p => p.ProductCategories.Select(pc => pc.Name).OrderBy(_ => _)));
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source);

            //ProductCategory
            CreateMap<ProductCategoryCreateRequest, ProductCategory>(MemberList.Source);
            CreateMap<ProductCategoryUpdateRequest, ProductCategory>(MemberList.Source);

            //WorkLoadItem
            CreateMap<WorkloadItem, WorkloadItemListViewModel>(MemberList.Source)
                .ForMember(_ => _.ProductId, opt => opt.MapFrom(_ => _.Product.Id))
                .ForMember(_ => _.ProductName, opt => opt.MapFrom(_ => _.Product.Name));
        }
    }
}
