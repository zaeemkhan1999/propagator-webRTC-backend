using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
    public class ProductQueries
    {
        [GraphQLName("product_getAllProducts")]
        public async Task<ListResponseBase<ProductsDto>> GetAllProducts(
                            [Authentication] Authentication authentication,
                            [Service] IProductReadService service)
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }
            return await service.GetAllProducts();
        }

        [GraphQLName("product_GetProductDetails")]
        public async Task<ListResponseBase<ProductsDto>> GetProductDetails(
                            [Authentication] Authentication authentication,
                            [Service] IProductReadService service,int Id)
        {
            if (authentication.Status != ResponseStatus.Success)
            {
                return authentication.Status;
            }
            return await service.GetProductDetails(Id);
        }
    }

