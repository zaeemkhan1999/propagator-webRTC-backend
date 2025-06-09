

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class UserDiscountRepository
 : Repository<UserDiscount,DataReadContext>,IUserDiscountRepository
{
public UserDiscountRepository(IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;

    #endregion
    #region functions

    public bool CheckIsDiscountAlreadyAppliedForUser(int userId, string discountcode)
    {
        var query = context.UserDiscount.Any<UserDiscount>(d => d.UserId == userId && d.Discount.DiscountCode == discountcode);

        return query;
    }
    #endregion
}
