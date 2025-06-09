

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserDiscountReadRepository
 : Repository<UserDiscount,DataWriteContext>, IUserDiscountReadRepository
{
public UserDiscountReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataWriteContext context;

    #endregion
    #region functions

    public bool CheckIsDiscountAlreadyAppliedForUser(int userId, string discountcode)
    {
        var query = context.UserDiscount.Any<UserDiscount>(d => d.UserId == userId && d.Discount.DiscountCode == discountcode);

        return query;
    }
    #endregion
}
