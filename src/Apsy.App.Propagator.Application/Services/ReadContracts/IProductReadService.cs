using Apsy.App.Propagator.Domain.Common.Inputs;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IProductReadService : IServiceBase<Product, ProductInput>
    {
        Task<ListResponseBase<ProductsDto>> GetAllProducts();
        Task<ListResponseBase<ProductsDto>> GetProductDetails(int Id);
    }
}
