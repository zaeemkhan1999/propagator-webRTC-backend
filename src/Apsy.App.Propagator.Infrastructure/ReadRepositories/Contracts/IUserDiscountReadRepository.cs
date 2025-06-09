namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IUserDiscountReadRepository : IRepository<UserDiscount>
{
    bool CheckIsDiscountAlreadyAppliedForUser(int userId, string discountcode);

}
