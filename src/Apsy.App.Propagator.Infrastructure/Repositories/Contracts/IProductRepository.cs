using OpenSearch.Net;

namespace Apsy.App.Propagator.Infrastructure.Repositories.Contracts
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<ProductImages> ProductImagesAsync(ProductImages productImages);
        Task<Product> AddAsync(Product product);

        Task<Product> UpdateProductAsync(Product product);

        Task<Reviews> AddReviewAsync(Reviews review);

        Task<Product> GetProductDetails(int productId);

        Task<List<ProductImages>> GetProductImages(int productId);
        Task DeleteProductImages(List<int> ProductImageIdes);
    }
}

