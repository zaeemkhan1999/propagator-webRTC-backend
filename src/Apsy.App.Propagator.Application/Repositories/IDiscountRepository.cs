using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Application.Repositories;

public interface IDiscountRepository : IRepository<Discount>
{
   IQueryable<Discount> GetDiscount(int id);
    IQueryable<Discount> GetDiscount();
    IQueryable<Discount> GetDiscountByCode(string discountcode);

}
