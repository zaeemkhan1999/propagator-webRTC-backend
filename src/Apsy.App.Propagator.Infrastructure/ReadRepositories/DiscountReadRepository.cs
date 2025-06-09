

using Aps.CommonBack.Base.Generics.Responses;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class DiscountReadRepository
 : Repository<Discount,DataWriteContext>, IDiscountReadRepository
{
    
public DiscountReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	public DataWriteContext context;

    #endregion
    #region functions

    public IQueryable<Discount> GetDiscount(int id)
    {
        var query = context.Discount.Where(x=>x.Id==id).AsQueryable();

        return query;
    }
    public IQueryable<Discount> GetDiscount()
    {
        var query = context.Discount.AsQueryable();

        return query;
    }

    public IQueryable<Discount> GetDiscountByCode(string discountcode)
    {
        var query = context.Discount.Where(x => x.DiscountCode == discountcode).AsNoTracking().AsQueryable();

        return query;
    }

    #endregion
}
