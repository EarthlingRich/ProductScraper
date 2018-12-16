using Api.Models;
using AutoMapper;
using Model;
using Model.Models;

namespace Api.Services
{
    public class ProductCategoryService
    {
        readonly IMapper _mapper;
        readonly ApplicationContext _context;

        public ProductCategoryService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void Create(ProductCategoryCreateRequest request)
        {
            var productCategory = _mapper.Map<ProductCategory>(request);
            _context.ProductCategories.Add(productCategory);
            _context.SaveChanges();
        }

        public void Update(ProductCategoryUpdateRequest request)
        {
            var productCategory = _context.ProductCategories.Find(request.Id);
            _mapper.Map(request, productCategory);
            _context.SaveChanges();
        }
    }
}
