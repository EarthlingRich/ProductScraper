using System.Linq;
using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            //Ingredient
            CreateMap<IngredientUpdateRequest, Ingredient>(MemberList.Source);
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
        }
    }
}
