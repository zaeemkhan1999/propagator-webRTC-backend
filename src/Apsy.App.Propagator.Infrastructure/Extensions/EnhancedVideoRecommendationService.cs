using Apsy.App.Propagator.Infrastructure.Repositories;
using LinqKit;
using System.Text.RegularExpressions;

namespace Apsy.App.Propagator.Infrastructure.Extensions;

public class EnhancedVideoRecommendationService
{

    private IPostReadRepository _repository;
    private User _currentUser;


    public async Task<List<PostDto>> RecommendPostsForUser(IPostReadRepository repository, User currentUser)
    {
        _currentUser = currentUser;
        _repository = repository;

        var recommendedPosts = new List<PostDto>();
        //00
        if (recommendedPosts.Count < 20)
        {
            recommendedPosts.AddRange(GetWithLatestAction(currentUser.Id, 20));
        }


        return await Task.Run(() => recommendedPosts);
    }
    public List<PostDto> RecommendPostsForUserForHanlder(IPostReadRepository repository, User currentUser)
    {
        _currentUser = currentUser;
        _repository = repository;

        var recommendedPosts = new List<PostDto>();
        //00
        if (recommendedPosts.Count < 20)
        {
            recommendedPosts.AddRange(GetWithLatestAction(currentUser.Id, 20));
        }

        return recommendedPosts;
    }

    private List<PostDto> GetWithLatestAction(int userId, int count)
    {
        try
        {
            // Get the list of posts with their latest interaction dates  

            var allActions = _repository.GetDbSet<Post>().Where(w => !w.IsDeleted && w.PosterId == userId).Select(share => new { share.Id, share.CreatedDate, actionType = "Share" })
                  .Concat(_repository.GetDbSet<PostLikes>().Where(w => !w.IsDeleted && w.UserId == userId).Select(like => new { Id = like.PostId, like.CreatedDate, actionType = "Like" }))
                  .Concat(_repository.GetDbSet<Comment>().Where(w => !w.IsDeleted && w.UserId == userId).Select(comment => new { Id = comment.PostId, CreatedDate = comment.CreatedAt, actionType = "Comment" }))
                  .Concat(_repository.GetDbSet<UserViewPost>().Where(w => !w.IsDeleted && w.UserId == userId).Select(view => new { Id = view.PostId, view.CreatedDate, actionType = "View" }))
                  .ToList();
            var latestAction = allActions.OrderByDescending(action => action.CreatedDate).FirstOrDefault();
            var postsWithInteractions = _repository.GetDbSet()
                .Where(p => !p.IsDeleted) // Filter out deleted posts  
                .Select(p => new
                {
                    Post = p
                })
                .ToList(); // Execute the query and pull into memory  
            var latestPost = postsWithInteractions
               .Where(p => p.Post.Id == latestAction.Id)
               .Select(x => new { x.Post, latestAction.actionType })
               .FirstOrDefault();


            // Convert the anonymous type list to a tuple list
            var actions = allActions
                .Select(a => (a.Id, a.CreatedDate, a.actionType))
                .ToList();
            var result16 = GetHigestRankedPosts(count, latestPost.Post, latestPost.actionType);
            var result4 = GetMostPopularFromGeneral(result16);
            var mergedResults = result16.Concat(result4).ToList();
            return mergedResults;
        }


        catch (Exception)
        {

            return new List<PostDto>();
        }
    }

    private List<PostDto> GetMostPopularFromGeneral(List<PostDto> result16)
    {

        var result16Ids = result16.Select(p => p.Post.Id).ToList();

        var dbSet = _repository.GetDbSet<Post>()
            .Include(i => i.UserViewPosts)
            .Include(i => i.Poster)
            .Include(i => i.Likes)
            .Include(i => i.NotInterestedPosts)
            .Include(i => i.Comments).ThenInclude(i => i.LikeComments)
            .Where(p => !p.IsDeleted && !result16Ids.Contains(p.Id));

        var result4 = dbSet
            .OrderByDescending(p => p.LikesCount + p.CommentsCount + p.ShareCount + p.UserViewPosts.Count)
            .Take(4)
            .Select(MapList)
            .ToList();

        return result4;
    }

    private List<PostDto> GetHigestRankedPosts(int count, Post latestPost, string actionType)
    {
        if (latestPost == null)
        {
            return new List<PostDto>();
        }
        var lastPostYourMindLower = latestPost.YourMind.ToLower();
        List<string> hashtags = new List<string>();
        Regex regex = new Regex(@"#(\w+)");
        MatchCollection matches = regex.Matches(lastPostYourMindLower);
        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                var item = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(item))
                {
                    hashtags.Add(item);
                }
            }
        }
        var dbSet = _repository.GetDbSet()
            .Include(i => i.UserViewPosts)
            .Include(i => i.Poster)
            .Include(i => i.Likes)
            .Include(i => i.NotInterestedPosts)
            .Include(i => i.Comments).ThenInclude(i => i.LikeComments)
            .Where(p => !p.IsDeleted && !string.IsNullOrWhiteSpace(p.YourMind));
        if (matches.Count > 0)
        {
            var result16 = FilterByWords(dbSet, hashtags.ToList())
            .OrderByDescending(p => p.LikesCount + p.CommentsCount + p.ShareCount + p.UserViewPosts.Count)
            .Take(16)
            .Select(MapList)
            .ToList();
            return result16;
        }
        else
        {
            var lastPostWords = lastPostYourMindLower
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.ToLower())
            .ToList();

            if (lastPostWords == null) return new List<PostDto>();
            var result = FilterByWords(dbSet, lastPostWords)
            .OrderByDescending(p => p.LikesCount + p.CommentsCount + p.ShareCount + p.UserViewPosts.Count)
            .Take(16)
            .Select(MapList)
            .ToList();

            return result;

        }



    }

    public IQueryable<Post> FilterByWords(IQueryable<Post> dbSet, List<string> lastPostWords)
    {
        var predicate = PredicateBuilder.New<Post>(true); // Start with a true predicate
        foreach (var word in lastPostWords)
        {
            var temp = word.ToLower();
            predicate = predicate.Or(x => x.YourMind.ToLower().Contains(temp));
        }
        // Apply the predicate
        var filteredQuery = dbSet.AsExpandable().Where(predicate);
        return filteredQuery;
    }



    public Expression<Func<Post, PostDto>> MapList
    {
        get
        {

            return (post) => new PostDto()
            {
                Post = post,
                PostItemsString = post.PostItemsString,
                IsLiked = post.Likes.Any(c => c.UserId == _currentUser.Id),
                IsViewed = post.UserViewPosts.Any(c => c.UserId == _currentUser.Id),
                IsNotInterested = post.NotInterestedPosts.Any(c => c.UserId == _currentUser.Id),
                IsSaved = post.SavePosts.Any(c => c.UserId == _currentUser.Id),
                IsYourPost = post.PosterId == _currentUser.Id,
                CommentCount = post.CommentsCount,
                ShareCount = post.ShareCount,
                LikeCount = post.LikesCount,
                ViewCount = post.Hits,
                PosterFollowerCount = post.Poster.Followers.Count(),
                NotInterestedPostsCount = post.NotInterestedPostsCount,
                PostComments = post.Comments.OrderByDescending(c => c.LikeCount).Take(3)
                                    .Select(c => new CommentDto
                                    {
                                        Comment = c,
                                        IsLiked = c.LikeComments.Any(c => c.UserId == _currentUser.Id),
                                        LikeCount = c.LikeCount,
                                        HasChild = c.Children.Any(),
                                        ChildrenCount = c.Children.Count
                                    }).ToList()
            };
        }
    }
    public  /*static*/ Expression<Func<Post, bool>> WhereList
    {
        get
        {

            _currentUser.NotInterestedPostIds ??= new List<int>();

            return (post) => !post.Poster.Blocks.Any(c => c.BlockedId == _currentUser.Id) && !post.Reports.Any(c => c.ReporterId == _currentUser.Id) &&
                         !post.Poster.Blockers.Any(c => c.BlockerId == _currentUser.Id) && post.IsCreatedInGroup == false &&
                         (!post.Poster.PrivateAccount || post.Poster.Followers.Any(x => x.FollowerId == _currentUser.Id)) &&
                        !_currentUser.NotInterestedPostIds.Any(x => post.Id == x) && post.Poster.IsDeletedAccount == false;
        }
    }


}