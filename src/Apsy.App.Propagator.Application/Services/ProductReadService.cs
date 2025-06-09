using Apsy.App.Propagator.Application.Services.ReadContracts;
using Apsy.App.Propagator.Domain.Common.Inputs;
using Apsy.App.Propagator.Infrastructure.ReadRepositories.Contracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class ProductReadService : ServiceBase<Product, ProductInput>, IProductReadService
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IProductReadRepository _productRepository;

        public ProductReadService(
            IFileUploadService fileUploadService,
            IProductReadRepository repository
        ) : base(repository)
        {
            _fileUploadService = fileUploadService;
            _productRepository = repository;
        }
        public async Task<ListResponseBase<ProductsDto>> GetAllProducts()
        {
            var products = await _productRepository.GetProductsAsync();
            return new ListResponseBase<ProductsDto>(products.AsQueryable());
        }

        public async Task<ListResponseBase<ProductsDto>> GetProductDetails(int Id)
        {
            var products = await _productRepository.GetProductDetails(Id);
            return new ListResponseBase<ProductsDto>(products.AsQueryable());
        }
    }
}
