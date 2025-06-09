using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IDiscountReadRepository : IRepository<Discount>
{
   IQueryable<Discount> GetDiscount(int id);
    IQueryable<Discount> GetDiscount();
    IQueryable<Discount> GetDiscountByCode(string discountcode);

}
