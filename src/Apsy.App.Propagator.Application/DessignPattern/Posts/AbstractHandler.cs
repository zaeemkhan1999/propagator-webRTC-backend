

namespace Apsy.App.Propagator.Application.DessignPattern.Posts;

// The default chaining behavior can be implemented inside a base handler
// class.
public class AbstractHandler : IHandler
{
    public AbstractHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private IHandler _nextHandler;
    protected readonly IHttpContextAccessor _httpContextAccessor;

    public IHandler SetNext(IHandler handler)
    {
        _nextHandler = handler;

        // Returning a handler from here will let us link handlers in a
        // convenient way like this:
        // monkey.SetNext(squirrel).SetNext(dog);
        return handler;
    }

    public virtual ListResponseBase<PostDto> Handle(object request,User currentUser)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Handle(request, currentUser);
        }
        else
        {
            return null;
        }
    }


    public  /*static*/ Expression<Func<Post, bool>> WhereList
    {
        get
        {
            var currentUser = GetCurrentUser();
            currentUser.NotInterestedPostIds ??= new List<int>();

            return (post) => !post.Poster.Blocks.Any(c => c.BlockedId == currentUser.Id) && !post.Reports.Any(c => c.ReporterId == currentUser.Id) &&
                         !post.Poster.Blockers.Any(c => c.BlockerId == currentUser.Id) && post.IsCreatedInGroup == false &&
                         (!post.Poster.PrivateAccount || post.Poster.Followers.Any(x => x.FollowerId == currentUser.Id)) &&
                        !currentUser.NotInterestedPostIds.Any(x => post.Id == x) && post.Poster.IsDeletedAccount == false;
        }
    }


    
    public  /*static*/ Expression<Func<Post, PostDto>> MapList
    {
        get
        {
            var currentUser = GetCurrentUser();
            return (post) => new PostDto()
            {
                Post = post,
                PostItemsString = post.PostItemsString,
                IsLiked = post.Likes.Any(c => c.UserId == currentUser.Id),
                IsViewed = post.UserViewPosts.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = post.NotInterestedPosts.Any(c => c.UserId == currentUser.Id),
                IsSaved = post.SavePosts.Any(c => c.UserId == currentUser.Id),
                IsYourPost = post.PosterId == currentUser.Id,
                CommentCount = post.CommentsCount,
                ShareCount = post.ShareCount,
                LikeCount = post.LikesCount,
                ViewCount = post.Hits,
                NotInterestedPostsCount = post.NotInterestedPostsCount,
                PostComments = post.Comments.OrderByDescending(c => c.LikeCount).Take(3)
                                    .Select(c => new CommentDto
                                    {
                                        Comment = c,
                                        IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                                        LikeCount = c.LikeCount,
                                        HasChild = c.Children.Any(),
                                        ChildrenCount = c.Children.Count
                                    }).ToList()
            };
        }
    }
    public Expression<Func<Post, double>> OrderList
    {
        get
        {
            return (post) =>
                         /*post.PromoteOrAdsPriceScore +*/
                         (post.ThisWeekHits + post.ThisWeekSavePostsCount + post.ThisWeekCommentsCount * 1.2 + post.ThisWeekLikesCount + post.ThisWeekShareCount - post.ThisWeekNotInterestedPostsCount) * 10 +
                         (post.Hits + post.SavePostsCount + post.CommentsCount * 1.2 + post.LikesCount + post.ShareCount - post.NotInterestedPostsCount);
        }
    }


    public User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

}
