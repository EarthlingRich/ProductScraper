using AutoMapper;
using Model.Models;
using Model.Requests;

namespace Model
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
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source);
            CreateMap<ProductStoreRequest, Product>(MemberList.Source)
                .ForSourceMember(_ => _.ProductCategory, opt => opt.DoNotValidate());

            //ProductCategory
            CreateMap<ProductCategoryCreateRequest, ProductCategory>(MemberList.Source);
            CreateMap<ProductCategoryUpdateRequest, ProductCategory>(MemberList.Source);
        }
    }
}
