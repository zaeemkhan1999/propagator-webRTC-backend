using GraphQL;
using Discount = Apsy.App.Propagator.Domain.Entities.Discount;

namespace Apsy.App.Propagator.Application.Services;

public class DiscountService : ServiceBase<Discount, DiscountInput>, IDiscountService
{
    public DiscountService(
        IDiscountRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
    }
    private readonly IDiscountRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    public async Task<ResponseBase<Discount>> AddDiscount(DiscountInput input)
    {
        var discount = input.Adapt<Discount>();

        await repository.AddAsync(discount);

        return ResponseBase<Discount>.Success(discount);
    }

    public async Task<ResponseBase<Discount>> UpdateDiscount(DiscountInput input)
    {
        var query = repository.GetDiscount((int)input.Id);

        if (input.Id == 0 || !query.Any())
            return ResponseStatus.NotFound;

        var discount = await query.FirstOrDefaultAsync();
        input.Adapt(discount);

        await repository.UpdateAsync(discount);

        return ResponseBase<Discount>.Success(discount);
    }

    public async Task<ResponseBase> DeleteDiscount(int Id)
    {
        var query = repository.GetDiscount(Id);

        if (Id == 0 || !query.Any())
            return ResponseStatus.NotFound;

        var discount = await query.FirstOrDefaultAsync();

        await repository.RemoveAsync(discount);

        return ResponseBase.Success();
    }
}

