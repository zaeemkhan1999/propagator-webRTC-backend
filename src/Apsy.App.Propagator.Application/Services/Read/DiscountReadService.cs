using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class DiscountReadService : ServiceBase<Discount, DiscountInput>, IDiscountReadService
    {
        private readonly IDiscountReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        public DiscountReadService(
        IDiscountReadRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _publisher = publisher;
        }
        public SingleResponseBase<Discount> GetDiscount(int id)
        {
            var query = repository.GetDiscount(id);

            return SingleResponseBase.Success(query);
        }

        public ListResponseBase<Discount> GetDiscounts()
        {
            var query = repository.GetDiscount();

            return ListResponseBase.Success(query);
        }
    }
}
