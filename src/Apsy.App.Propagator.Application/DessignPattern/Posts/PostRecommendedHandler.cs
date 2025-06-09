using Aps.CommonBack.Base.Repositories;
using Aps.CommonBack.Base.Repositories.Contracts;
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Propagator.Common.Services.Contracts;
using Stripe;

namespace Apsy.App.Propagator.Application.DessignPattern.Posts;

public class PostRecommendedHandler : AbstractHandler
{
    private readonly IPostReadRepository repository;
    private readonly IPostReadService postService;
    private readonly IUsersSubscriptionReadService _usersSubscriptionService;

    public PostRecommendedHandler(IPostReadService postReadService, IPostReadRepository repository, IUsersSubscriptionReadService usersSubscriptionService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
        this.postService = postReadService;
        _usersSubscriptionService = usersSubscriptionService;
    }

    public override ListResponseBase<PostDto> Handle(object request,User currentUser)
    {
        if ((GetPostType)request == GetPostType.Recommended)
        {
            
            var recommendedPosts = postService.GetRecommendedPostsForHandler().ToList();
            currentUser.NotInterestedPostIds ??= new List<int>();
            var subscriptionsFeature = _usersSubscriptionService.GetUsersSubscriptionsFeatures().Result;
            bool isRemoveAds = subscriptionsFeature.Status == ResponseStatus.Success && subscriptionsFeature.Result.RemoveAds;

            //newest posts
            var takeNewest = repository.Newest(currentUser, isRemoveAds).ToList();

            //my posts
            var takmypost = repository.MyPosts(currentUser, isRemoveAds).ToList();
            foreach (var item in takeNewest)
            {
                if (item.Post.Poster is null)
                {
                    item.Post.Poster = repository.GetUserByPosterId(item.Post.PosterId);
                }
            }

            var combinedPosts = recommendedPosts.Concat(takeNewest).Concat(takmypost).DistinctBy(x=>x.Post.Id).AsQueryable();
            return new(combinedPosts);
        }
        else
        {
            return base.Handle(request,currentUser);
        }


    }
}