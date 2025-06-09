namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IDiscountService : IServiceBase<Discount, DiscountInput>
{
    Task<ResponseBase<Discount>> AddDiscount(DiscountInput input);
    Task<ResponseBase<Discount>> UpdateDiscount(DiscountInput input);
    Task<ResponseBase> DeleteDiscount(int id);
}
