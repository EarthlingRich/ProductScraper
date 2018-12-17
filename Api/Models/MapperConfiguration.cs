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
            CreateMap<Product, ProductUpdateViewModel>()
                .ForMember(_ => _.Request, opt => opt.Ignore());
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source);

            //ProductCategory
            CreateMap<ProductCategoryCreateRequest, ProductCategory>(MemberList.Source);
            CreateMap<ProductCategoryUpdateRequest, ProductCategory>(MemberList.Source);
        }
    }
}
