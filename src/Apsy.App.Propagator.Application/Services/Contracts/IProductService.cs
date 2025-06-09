using Apsy.App.Propagator.Domain.Common.Inputs;
using Apsy.Common.Api.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Contracts
{
   public interface IProductService: IServiceBase<Product, ProductInput>
    {
        Task<Product> CreateProductAsync(ProductInput input);

        Task<Product> UpdateProductAsync(UpdateProductInput input);

        Task<Reviews> AddProductReview(ReviewInput input);

        Task<Product> GetProductAsync(int productId);
    }
}
