using AutoMapper;
using Model.Models;

namespace Api.Models
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<Product, ProductUpdateViewModel>()
                .ForMember(_ => _.Request, opt => opt.Ignore());
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source);

            CreateMap<IngredientUpdateRequest, Ingredient>(MemberList.Source);
            CreateMap<IngredientCreateRequest, Ingredient>(MemberList.Source);

            CreateMap<ProductCategoryCreateRequest, ProductCategory>(MemberList.Source);
            CreateMap<ProductCategoryUpdateRequest, ProductCategory>(MemberList.Source);
        }
    }
}
