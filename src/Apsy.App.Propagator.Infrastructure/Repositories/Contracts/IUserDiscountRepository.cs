namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserDiscountRepository : IRepository<UserDiscount>
{
    bool CheckIsDiscountAlreadyAppliedForUser(int userId, string discountcode);

}
