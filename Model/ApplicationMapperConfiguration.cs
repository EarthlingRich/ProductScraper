using System.Linq;
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
                .ForMember(_ => _.AllergyKeywordsString, opt => opt.MapFrom(i => i.AllergyKeywordsString.ToLower()))
                .ForMember(_ => _.IgnoreKeywordsString, opt => opt.MapFrom(i => i.IgnoreKeywordsString.ToLower()))
                .ForMember(_ => _.KeywordsString, opt => opt.MapFrom(i => i.KeywordsString.ToLower()));
            CreateMap<IngredientCreateRequest, Ingredient>(MemberList.Source);

            //Product
            CreateMap<ProductUpdateRequest, Product>(MemberList.Source)
                .ForMember(_ => _.WorkloadItems, opt => opt.Ignore())
                .AfterMap(UpdateList);
            CreateMap<ProductStoreRequest, Product>(MemberList.Source)
                .ForSourceMember(_ => _.ProductCategory, opt => opt.DoNotValidate());

            //ProductCategory
            CreateMap<ProductCategoryCreateRequest, ProductCategory>(MemberList.Source);
            CreateMap<ProductCategoryUpdateRequest, ProductCategory>(MemberList.Source);
        }

        private void UpdateList(ProductUpdateRequest request, Product product)
        {
            foreach(var updatedWorkloadItem in  request.WorkloadItems)
            {
                var exisitingWorloadItem = product.WorkloadItems.Single(_ => _.Id == updatedWorkloadItem.Id);
                exisitingWorloadItem.IsProcessed = updatedWorkloadItem.IsProcessed;
            }
        }
    }
}
