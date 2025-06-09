namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PostLikeRepository : Repository<PostLikes, DataReadContext>, IPostLikeRepository
{

    public PostLikeRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private readonly DataReadContext context;
    #endregion

    #region functions
    public IQueryable<PostLikes> GetAllPostLikes()
    {
        var query = context.PostLikes.AsQueryable();
        return query;
    }
    public IQueryable<PostLikesDto> GetPostLikesByUser(int userId)
    {
        var postlike = context.PostLikes.Where(c => !c.Post.Poster.Blocks.Any(c => c.BlockedId == userId) && c.UserId == userId)
                .Select(c => new PostLikesDto
                {
                    Id = c.Id,
                    CreatedDate = c.CreatedDate,
                    Post = c.Post,
                    //PostItems = JsonConvert.DeserializeObject<List<PostItem>>(c.PostItemsString).ToList(),
                    PostItemsString = c.Post.PostItemsString,
                    IsLiked = c.Post.Likes.Any(c => c.UserId == userId && c.Liked),
                    IsViewed = c.Post.UserViewPosts.Any(c => c.UserId == userId),
                    IsNotInterested = c.Post.NotInterestedPosts.Any(c => c.UserId == userId),
                    IsSaved = c.Post.SavePosts.Any(c => c.UserId == userId),
                    IsYourPost = c.Post.PosterId == userId,
                    CommentCount = c.Post.Comments.Count,
                    ShareCount = c.Post.Messages.Count,
                    LikeCount = c.Post.LikesCount,
                    ViewCount = c.Post.UserViewPosts.Count,
                }).AsNoTracking().AsQueryable();
        return postlike;
    }

    public IQueryable<PostLikesDto> GetPostLikesUsers(int postId)
    { 
        var postlike = context.PostLikes.Where(c => c.Post.Likes.Any() && c.PostId == postId) 
                    .Select(c => new PostLikesDto
                    {
                        Id=c.Id,
                        User=c.User
                    });

        return postlike;
    }

    public int GetTotalPostLikesCount()
    {
        return context.PostLikes.Count(x => x.Liked);
    }

    public PostLikes GetSinglePostLikesByPost(int postId, int userId)
    {
      return  context.PostLikes.Where((PostLikes a) => a.PostId == postId && a.UserId == userId).FirstOrDefault();
    }
    #endregion
}

