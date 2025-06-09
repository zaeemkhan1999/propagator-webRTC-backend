namespace Apsy.App.Propagator.Infrastructure.ReadRepositories.Contracts
{
    public interface IProductReadRepository : IRepository<Product>
    {
        Task<List<ProductsDto>> GetProductsAsync();
        Task<List<ProductsDto>> GetProductDetails(int id);
    }
}
