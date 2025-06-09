using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.DessignPattern.Posts;

public class PostForYouHandler : AbstractHandler
{
    private readonly IPostReadRepository repository;
    private readonly IUsersSubscriptionReadService _usersSubscriptionService;

    public PostForYouHandler(IPostReadRepository repository, IUsersSubscriptionReadService usersSubscriptionService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
        _usersSubscriptionService = usersSubscriptionService;
    }

    public override ListResponseBase<PostDto> Handle(object request,User currentUser)
    {
        currentUser.NotInterestedPostIds ??= new List<int>();
        var subscriptionsFeature = _usersSubscriptionService.GetUsersSubscriptionsFeatures().Result;
        bool isRemoveAds = subscriptionsFeature.Status == ResponseStatus.Success && subscriptionsFeature.Result.RemoveAds;


        if ((GetPostType)request == GetPostType.ForYou)
        {
            var result = repository.ForYou(currentUser, isRemoveAds);
            return new(result);
        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }
}
