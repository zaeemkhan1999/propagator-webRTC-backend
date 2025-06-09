namespace Apsy.App.Propagator.Application.Repositories;

public interface IUserDiscountRepository : IRepository<UserDiscount>
{
    bool CheckIsDiscountAlreadyAppliedForUser(int userId, string discountcode);

}
