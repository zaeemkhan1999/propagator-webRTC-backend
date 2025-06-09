using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read;

public class SubscriptionPlanReadService : ServiceBase<SubscriptionPlan, SubscriptionPlanInput>, ISubscriptionPlanReadService
{
    private readonly ISubscriptionPlanReadRepository _repository;

    public SubscriptionPlanReadService(ISubscriptionPlanReadRepository repository)
        : base(repository)
    {
        _repository = repository;
    }

    public int GetSubscriptionPlanIdByPriceId(string priceId)
    {
        var planDetails = _repository.GetSubscriptionPlanByPriceId(priceId);
        return planDetails.Id;
    }

    public async Task<ListResponseBase<SubscriptionPlanDto>> GetSubscriptionPlansInDtoAsync()
    {
        var list = await _repository
            .Where(x => x.IsActive && !x.IsDeleted)
            .ToListAsync();

        return new ListResponseBase<SubscriptionPlanDto>(list.Select(x => new SubscriptionPlanDto
        {
            Id = x.Id,
            AddedToCouncilGroup = x.AddedToCouncilGroup,
            AllowDownloadPost = x.AllowDownloadPost,
            Price = x.Price,
            PriceId = x.PriceId,
            RemoveAds = x.RemoveAds,
            Supportbadge = x.Supportbadge,
            Title = x.Title,

            Content = JsonConvert.DeserializeObject<SubscriptionPlanContentDto>(x.Content)
        }).AsQueryable());
    }
}
