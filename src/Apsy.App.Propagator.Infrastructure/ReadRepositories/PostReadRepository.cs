//using Firebase.Auth;
using Aps.CommonBack.Base.Repositories;
using Aps.CommonBack.Base.Repositories.Contracts;
using System.Linq;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class PostReadRepository : Repository<Post, DataWriteContext>, IPostReadRepository
//: PostRepositoryBase<User, Post, PostLikes, Comment, DataWriteContext>, IPostRepository
{
    public PostReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }
    private readonly DataWriteContext context;

    public async Task<ResponseBase<PostWatchHistory>> AddWatchHistory(int id, int UserId)
    {

        var post = await GetByIdAsync(id);
        if (post == null)
            throw new Exception("Post not fount");

        var PostWatch = new PostWatchHistory();
        {
            PostWatch.PostId = id;
            PostWatch.UserId = UserId;
            PostWatch.WatchDate = DateTime.Now;
        }
        await context.AddAsync(PostWatch);
        await context.SaveChangesAsync();
        return PostWatch;
    }

    public ResponseBase<List<PostWatchHistory>> GetWatchedHistory(int id)
    {
        var query = context.PostWatchHistory
            .Where(x => x.UserId == id)
            .Include(p => p.Post)
            .ToList();

        if (query == null || !query.Any())
        {
            return ResponseBase<List<PostWatchHistory>>.Failure(ResponseStatus.NotFound);
        }

        return ResponseBase<List<PostWatchHistory>>.Success(query);
    }
    public IQueryable<Post> GetPostById(int id)
    {
        var post = context.Post.Where(p => p.Id == id).AsNoTracking().AsQueryable();
        return post;
    }

    public IQueryable<Post> GetAllPosts()
    {
        var posts = context.Post.AsNoTracking().AsQueryable();
        return posts;
    }
    
    public IQueryable<UserViewPost> GetAllUserViewPost()
    {
        var query = context.UserViewPost.AsQueryable();
        return query;
    }
    

    public IQueryable<PostDto> GetPostByUser(int id, int userId)
    {
        var post = context.Post.Where(p => p.Id == id).Where(c => !c.Poster.Blocks.Any(c => c.BlockedId == userId));
        var result = post.Select(c => new PostDto()
             {
                 Post = c,
                 PostItemsString = c.PostItemsString,
                 IsLiked = c.Likes.Any(c => c.UserId == userId && c.Liked),
                 IsViewed = c.UserViewPosts.Any(c => c.UserId == userId),
                 IsNotInterested = c.NotInterestedPosts.Any(c => c.UserId == userId),
                 IsSaved = c.SavePosts.Any(c => c.UserId == userId),
                 IsYourPost = c.PosterId == userId,
                 CommentCount = c.Comments.Count,
                 ShareCount = c.Messages.Count,
                 LikeCount = c.LikesCount,
                 ViewCount = c.UserViewPosts.Count,
                 PostComments = c.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                         .Select(c => new CommentDto
                         {
                             Comment = c,
                             IsLiked = c.LikeComments.Any(c => c.UserId == userId),
                             HasChild = c.Children.Any(),
                             ChildrenCount = c.Children.Count,
                             LikeCount = c.LikeComments.Count
                         }).ToList()
             }).AsNoTracking().AsQueryable();
        return result;
    }
    public IQueryable<PostDto> GetPosts(User currentUser)
    {
        var post = context.Post.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Where(c => !c.Poster.Blocks.Any(c => c.BlockedId == currentUser.Id))
            .Where(c => c.Poster.IsDeletedAccount == false);
            var result = post.Select(c => new PostDto
            {
                Post = c,
                PostItemsString = c.PostItemsString,
                IsLiked = c.Likes.Any(c => c.UserId == currentUser.Id && c.Liked),
                IsViewed = c.UserViewPosts.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = c.NotInterestedPosts.Any(c => c.UserId == currentUser.Id),
                IsSaved = c.SavePosts.Any(c => c.UserId == currentUser.Id),
                IsYourPost = c.PosterId == currentUser.Id,
                CommentCount = c.CommentsCount,
                ShareCount = c.Messages.Count,
                LikeCount = c.LikesCount,
                ViewCount = c.Hits,
                NotInterestedPostsCount = c.NotInterestedPostsCount,
                PostComments = c.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                        .Select(c => new CommentDto
                        {
                            Comment = c,
                            IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                            HasChild = c.Children.Any(),
                            ChildrenCount = c.Children.Count,
                            LikeCount = c.LikeComments.Count
                        }).ToList()
            }).AsNoTracking().AsQueryable();
        return result;
    }
    public int GetPostCount(User currentUser)
{
    var postCount = context.Post
        .Where(d => d.DeletedBy == DeletedBy.NotDeleted && d.PosterId==currentUser.Id && d.IsDeleted==false)
        .Where(c => !c.Poster.IsDeletedAccount)
        .Count();

    return postCount;
}

    public IQueryable<PostDto> GetAdsPosts(User currentUser)
    {
        var post = context.Post.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Where(c => c.Poster.Blocks.All(x => x.BlockedId != currentUser.Id))
            .Where(c => c.Poster.IsDeletedAccount == false && c.Poster.ProfessionalAccount)
            .Where(x => x.PostType == PostType.Ads);
           var result = post.Select(c => new PostDto
            {
                Post = c,
                PostItemsString = c.PostItemsString,
                IsLiked = c.Likes.Any(x => x.UserId == currentUser.Id && x.Liked),
                IsViewed = c.UserViewPosts.Any(x => x.UserId == currentUser.Id),
                IsNotInterested = c.NotInterestedPosts.Any(x => x.UserId == currentUser.Id),
                IsSaved = c.SavePosts.Any(x => x.UserId == currentUser.Id),
                IsYourPost = c.PosterId == currentUser.Id,
                CommentCount = c.CommentsCount,
                ShareCount = c.Messages.Count,
                LikeCount = c.LikesCount,
                ViewCount = c.Hits,
                NotInterestedPostsCount = c.NotInterestedPostsCount,
                IsCompletedPayment = c.IsCompletedPayment,
                PostComments = c.Comments.OrderByDescending(x => x.LikeComments.Count).Take(3)
                        .Select(x => new CommentDto
                        {
                            Comment = x,
                            IsLiked = x.LikeComments.Any(c => c.UserId == currentUser.Id),
                            HasChild = x.Children.Any(),
                            ChildrenCount = x.Children.Count,
                            LikeCount = x.LikeComments.Count
                        }).ToList()
            }).AsNoTracking().AsQueryable();
        return result;
    }

    public IQueryable<PostDto> GetPromotedPosts(User currentUser)
    {
        var post = context.Post.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Where(c => c.Poster.Blocks.All(x => x.BlockedId != currentUser.Id))
            .Where(c => c.Poster.IsDeletedAccount == false && c.Poster.ProfessionalAccount)
            .Where(x => x.IsPromote && x.IsCompletedPayment);
           var result = post.Select(c => new PostDto
            {
                Post = c,
                PostItemsString = c.PostItemsString,
                IsLiked = c.Likes.Any(c => c.UserId == currentUser.Id && c.Liked),
                IsViewed = c.UserViewPosts.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = c.NotInterestedPosts.Any(c => c.UserId == currentUser.Id),
                IsSaved = c.SavePosts.Any(c => c.UserId == currentUser.Id),
                IsYourPost = c.PosterId == currentUser.Id,
                CommentCount = c.CommentsCount,
                ShareCount = c.Messages.Count,
                LikeCount = c.LikesCount,
                ViewCount = c.Hits,
                NotInterestedPostsCount = c.NotInterestedPostsCount,
                PostComments = c.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                        .Select(c => new CommentDto
                        {
                            Comment = c,
                            IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                            HasChild = c.Children.Any(),
                            ChildrenCount = c.Children.Count,
                            LikeCount = c.LikeComments.Count
                        }).ToList()
            }).AsNoTracking().AsQueryable();
        return result;
    }

    public IQueryable<PostDto> GetRandomPosts(User currentUser)
    {
        var post = context.Post.Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin &&
                                                                (d.PosterId == currentUser.Id ||
                                                                 currentUser.UserTypes == UserTypes.Admin ||
                                                                 currentUser.UserTypes == UserTypes.SuperAdmin)))
            .Where(c => !c.Poster.Blocks.Any(c => c.BlockedId == currentUser.Id))
            //.Where(c => !c.Reports.Any(c => c.ReportedId == currentUser.Id))
            .Where(c => c.Poster.IsDeletedAccount == false)
            .OrderBy(x => Guid.NewGuid());
           var result = post.Select(x => new PostDto
            {
                Post = x,
                //PostItems = JsonConvert.DeserializeObject<List<PostItem>>(c.PostItemsString).ToList(),
                PostItemsString = x.PostItemsString,
                IsLiked = x.Likes.Any(c => c.UserId == currentUser.Id && c.Liked),
                IsViewed = x.UserViewPosts.Any(c => c.UserId == currentUser.Id),
                IsNotInterested = x.NotInterestedPosts.Any(c => c.UserId == currentUser.Id),
                IsSaved = x.SavePosts.Any(c => c.UserId == currentUser.Id),
                IsYourPost = x.PosterId == currentUser.Id,
                CommentCount = x.CommentsCount,
                ShareCount = x.Messages.Count,
                LikeCount = x.LikesCount,
                ViewCount = x.Hits,
                NotInterestedPostsCount = x.NotInterestedPostsCount,
                PostComments = x.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                    .Select(c => new CommentDto
                    {
                        Comment = c,
                        IsLiked = c.LikeComments.Any(c => c.UserId == currentUser.Id),
                        HasChild = c.Children.Any(),
                        ChildrenCount = c.Children.Count,
                        LikeCount = c.LikeComments.Count
                    }).ToList()
            }).AsNoTracking().AsQueryable();
        return result;
    }

    public IQueryable<Post> GetExplorePostsAsyncBaseQuery(int? id, User currentUser, string searchTerm)
    {
        var post = context.Post.Where(x => string.IsNullOrEmpty(searchTerm) || x.YourMind.ToLower().Contains(searchTerm.ToLower()))
                .Where(x => !id.HasValue || x.Id < id)
                .Where(d => d.DeletedBy == DeletedBy.NotDeleted || (d.DeletedBy == DeletedBy.Admin &&
                                                                    (d.PosterId == currentUser.Id ||
                                                                     currentUser.UserTypes == UserTypes.Admin ||
                                                                     currentUser.UserTypes == UserTypes.SuperAdmin)))
                .Where(c => !c.Poster.Blocks.Any(c => c.BlockedId == currentUser.Id))
                //.Where(c => !c.Reports.Any(c => c.ReportedId == currentUser.Id))
                .Where(c => c.Poster.IsDeletedAccount == false).AsNoTracking().AsQueryable();
        return post;
    }

    public bool CheckIsNewPostAvailable(DateTime from)
    {
        bool isAvailable = false;

        var post = context.Post.Where(c => c.CreatedDate >= from && c.IsDeleted == false)
          .Where(c => c.Poster.IsDeletedAccount == false)
          .Any();

        return isAvailable;
    }

    public IQueryable<PostDto> GetTopPosts(int userId, DateTime from)
    {
        var post = context.Post.Where(c => c.CreatedDate >= from)
           .Where(c => c.Poster.IsDeletedAccount == false);
           var result = post.Select(c => new PostDto
           {
               Post = c,
               //PostItems = JsonConvert.DeserializeObject<List<PostItem>>(c.PostItemsString).ToList(),
               PostItemsString = c.PostItemsString,
               IsLiked = c.Likes.Any(c => c.UserId == userId && c.Liked),
               IsViewed = c.UserViewPosts.Any(c => c.UserId == userId),
               IsNotInterested = c.NotInterestedPosts.Any(c => c.UserId == userId),
               IsSaved = c.SavePosts.Any(c => c.UserId == userId),
               IsYourPost = c.PosterId == userId,
               CommentCount = c.CommentsCount,
               ShareCount = c.Messages.Count,
               LikeCount = c.LikesCount,
               ViewCount = c.Hits,
               NotInterestedPostsCount = c.NotInterestedPostsCount,
               PostComments = c.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                       .Select(c => new CommentDto
                       {
                           Comment = c,
                           IsLiked = c.LikeComments.Any(c => c.UserId == userId),
                           HasChild = c.Children.Any(),
                           ChildrenCount = c.Children.Count,
                           LikeCount = c.LikeComments.Count
                       }).ToList()
           }).OrderBy(c => (1.2 * c.CommentCount) + c.ShareCount + c.LikeCount + c.ViewCount - c.NotInterestedPostsCount).AsNoTracking().AsQueryable();
        return result;
    }

    public IQueryable<PostDto> GetFollowersPosts(int userId)
    {
        var post = context.UserFollower.Where(x => x.FollowerId == userId).Select(x => x.FollowedId).ToList();
            
                      
        var result = context.Post.Where(x=> post.Contains(x.PosterId))

                        
                        .Select(c => new PostDto
                        {
                            Post = c,
                            IsLiked = c.Likes.Any(c => c.UserId == userId && c.Liked),
                            IsViewed = c.UserViewPosts.Any(c => c.UserId == userId),
                            IsNotInterested = c.NotInterestedPosts.Any(c => c.UserId == userId),
                            IsSaved = c.SavePosts.Any(c => c.UserId == userId),
                            IsYourPost = c.PosterId == userId,
                            CommentCount = c.CommentsCount,
                            ShareCount = c.Messages.Count,
                            LikeCount = c.LikesCount,
                            ViewCount = c.Hits,
                            NotInterestedPostsCount = c.NotInterestedPostsCount,
                            PostComments = c.Comments.OrderByDescending(c => c.LikeComments.Count).Take(3)
                                .Select(c => new CommentDto
                                {
                                    Comment = c,
                                    IsLiked = c.LikeComments.Any(c => c.UserId == userId),
                                    HasChild = c.Children.Any(),
                                    ChildrenCount = c.Children.Count,
                                    LikeCount = c.LikeComments.Count
                                }).ToList()
                        });
            return result;
    }

    public int GetLikeCount(string content)
    {
        int likeCount = 0;
        likeCount = context.Post.Where(d => d.Tags != null && d.StringTags.Contains(content)).Sum(d => d.LikesCount);
        return likeCount;
    }

    public int GetTagUsesCount(string content)
    {
        int likeCount = 0;
        likeCount = context.Post.Where(d => d.Tags != null && d.StringTags.Contains(content)).Count();
        return likeCount;
    }

    public int? GetPostTaskId(int postId)
    {
        int? taskId = 0;
        taskId = context.Post.Where(x => x.Id == postId).Select(x => x.TaskId).First();
        return taskId;
    }

    public IQueryable<Ads> GetPostAds(int id)
    {
        var ads = context.Ads.Where(c => c.PostId == id).AsNoTracking().AsQueryable();
        return ads;
    }
    public IQueryable<SavePostDto> GetSavedPost(int userId)
    {
        var savePostDto = context.SavePost.Where(c => c.UserId == userId)
            .Where(c => !c.Post.Poster.Blocks.Any(c => c.BlockedId == userId))
            .Where(c => c.User.IsDeletedAccount == false);
            var result = savePostDto.Select(c => new SavePostDto
            {
                Post = c.Post,
                PostId = c.PostId,
                UserId = c.UserId,
                User = c.User,

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
        return result;
    }
    public IQueryable<PostLikes> GetPostLikesByUser(int userId)
    {
        var postLikes = context.PostLikes.Where(c => c.UserId == userId).AsNoTracking().AsQueryable();
        return postLikes;
    }

    public IQueryable<PostLikes> GetUserPostLikeWithPostDetails(int id, int userId)
    {
        var postLikes = context.PostLikes.Where(a => a.PostId == id && a.UserId == userId).Include(c => c.Post).AsNoTracking().AsQueryable();
        return postLikes;
    }
    public Task<List<Post>> GetPostsForAddViews(List<int> id, int userId)
    {
        var post = context.Post
            .Where(x => id.Contains(x.Id))
            .Include(x => x.UserViewPosts.Where(y => y.UserId == userId && y.CreatedDate.Date == DateTime.Now.Date))
            .Include(x => x.Adses.Where(y => y.IsCompletedPayment && y.IsActive))
            .ToListAsync();
        return post;
    }
    public int GetSinglePostLikesCount(int id)
    {
        var postLikes = context.PostLikes.Where(d => d.PostId == id && d.Liked).Count();
        return postLikes;
    }

    public int GetPostLikesCount()
    {
        var postLikes = context.PostLikes.Count();
        return postLikes;
    }
    public IQueryable<SavePost> GetSavePost(int id, int userId)
    {
        var post = context.SavePost.Where(a => a.PostId == id && a.UserId == userId).Include(c => c.Post).AsNoTracking().AsQueryable();
        return post;
    }

    public Post GetPostWithPoster(int id)
    {
        var post = context.Post
                    .Where(c => c.Id == id)
                    .Include(c => c.Poster).AsNoTracking()
                    .FirstOrDefault();

        return post;
    }

    public InterestedUser GetPostInterestedUser(int id, int userId, int posterId)
    {
        var userInterest = context.InterestedUser
                     .FirstOrDefault(x => x.FollowersUserId == posterId &&
                                               x.UserId == userId &&
                                               x.PostId == id &&
                                               x.InterestedUserType == InterestedUserType.Post);
        return userInterest;
    }

    public async Task<Post> UpdatePostsEngagement(Post post)
    {
        //DateTime startDateTime = DateTime.Today; //Today at 00:00:00
        DateTime sevenDaysAgo = DateTime.Today.AddDays(-7);

        var savedCount = await Context.Set<SavePost>().Where(c => c.PostId == post.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var likedCount = await Context.Set<PostLikes>().Where(c => c.PostId == post.Id && c.Liked && c.CreatedDate > sevenDaysAgo).CountAsync();
        var commentsCount = await Context.Set<Comment>().Where(c => c.PostId == post.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var hitsCount = await Context.Set<UserViewPost>().Where(c => c.PostId == post.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var sharedCount = await Context.Set<Message>().Where(c => c.MessageType == MessageType.Post && c.PostId == post.Id && c.CreatedDate > sevenDaysAgo).CountAsync();
        var notInterestedCount = await Context.Set<NotInterestedPost>().Where(c => c.PostId == post.Id && c.CreatedDate > sevenDaysAgo).CountAsync();

        post.LatestUpdateThisWeeks = DateTime.UtcNow;
        post.ThisWeekSavePostsCount = savedCount;
        post.ThisWeekLikesCount = likedCount;
        post.ThisWeekCommentsCount = commentsCount;
        post.ThisWeekHits = hitsCount;
        post.ThisWeekShareCount = sharedCount;
        post.ThisWeekNotInterestedPostsCount = notInterestedCount;

        return Update(post);
    }
    public async Task UpdatePostsCompreessedResponse(string PostItemString, int PostId)
    {
        var post = GetById(PostId).FirstOrDefault();
        post.PostItemsString = PostItemString;
        await Task.Run(()=> Update(post));
    }

    public async Task UpdatePostsCompreessedTaskId(int TaskId, int PostId)
    {
        var post = GetById(PostId).FirstOrDefault();
        post.TaskId = TaskId;
        await Task.Run(() => Update(post));
    }

    public int GetTotalUserPinPostCount(int userId)
    {
        var userPinPosts = context.Post.Where(c => c.PosterId == userId && c.IsPin).AsNoTracking().Count();
        return userPinPosts;
    }

    public bool CheckIsPostActiveAds(int postId)
    {
        var postActiveAds = context.Post.Any(c => c.Id == postId && c.Adses.Any(c => c.IsCompletedPayment && c.IsActive));
        return postActiveAds;
    }

    public IQueryable<PostDto> Explore(User currentUser,bool isRemoveAds)
    {
        var queryable = context.Post.Where(WhereList(currentUser));

        //where is Show to me !
        queryable = queryable
                 .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin));
        queryable = queryable.Where(c => c.PosterId != currentUser.Id);
        if (isRemoveAds)
        {
            queryable = queryable.Where(c =>
            c.PostType != PostType.Ads &&
                     (c.PostType == PostType.RegularPost ||
                     c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee)));
        }
        else
        {
            queryable = queryable.Where(c =>
                     c.PostType == PostType.RegularPost ||
                     c.PostType == PostType.Ads && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee)
                     ||
                     c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee));
        }

        queryable =
                queryable
                .OrderByDescending(c => c.LatestUpdateThisWeeks > DateTime.Today.AddDays(-7))
                .ThenByDescending(OrderList)
                .ThenByDescending(c => c.CreatedDate);

        var result = queryable.Select(MapList(currentUser));
        return result;
    }

    public IQueryable<PostDto> ForYou(User currentUser, bool isRemoveAds)
    {
        var queryable = context.Post.Where(WhereList(currentUser)); 
        
        queryable = queryable.Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin));
        if (isRemoveAds)
            queryable = queryable.Where(c => c.PostType != PostType.Ads);

        var iu = context.InterestedUser.Where(d => d.InterestedUserType == InterestedUserType.Post);
        var result = queryable.Select(post => new PostDto
        {
            Post = post,
            IsInterested = iu.Any(d => d.UserId == post.PosterId),
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
        }).OrderByDescending(d => d.IsInterested).ThenByDescending(d => d.Post.CreatedDate).ThenBy(d => Guid.NewGuid());

        return result;
    }

    public IQueryable<PostDto> MostEngaged(User currentUser, bool isRemoveAds)
    {
        var queryable = context.Post.Where(WhereList(currentUser));
        if (isRemoveAds)
            queryable = queryable.Where(c => c.PostType != PostType.Ads);

        //where is Show to me !
        queryable = queryable.Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin));
        queryable = queryable.Where(c => c.PosterId != currentUser.Id);
        if (isRemoveAds)
        {
            queryable =
                        queryable.Where(c =>
                                c.PostType != PostType.Ads && (
                                 c.PostType == PostType.RegularPost
                                 ||
                                 c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee)));
        }
        else
        {
            queryable = queryable.Where(c =>
                 c.PostType == PostType.RegularPost ||
                 c.PostType == PostType.Ads && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee)
                 ||
                 c.IsPromote && c.IsCompletedPayment && c.Adses.Any(c => c.IsActive && c.TotalViewed < c.NumberOfPeopleCanSee));
        }

        queryable = queryable
                        .OrderByDescending(c => c.LatestUpdateThisWeeks > DateTime.Today.AddDays(-7))
                        .ThenByDescending(OrderList)
                        .ThenByDescending(c => c.CreatedDate);

        var result = queryable.Select(MapList(currentUser));
        return result;

    }

    public IQueryable<PostDto> MyPosts(User currentUser, bool isRemoveAds)
    {
        var queryable = context.Post
                .Where(WhereList(currentUser))
                .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
                .Where(c => c.PosterId == currentUser.Id);

        if (isRemoveAds)
            queryable = queryable.Where(x => x.PostType != PostType.Ads);

        var result = queryable.Select(MapList(currentUser));
        return result;  
    }

    public IQueryable<PostDto> Newest(User currentUser, bool isRemoveAds)
    {
        var queryable = context.Post
              .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
              .Where(WhereList(currentUser));
        if (isRemoveAds)
            queryable = queryable.Where(x => x.PostType != PostType.Ads);
        queryable = queryable.OrderByDescending(c => c.CreatedDate);

        var result = queryable.Select(MapList(currentUser));
        return result;  
    }

    public IQueryable<PostDto> News(User currentUser, bool isRemoveAds)
    {
        var queryable = context.Post
                        .Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)).Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin)).Where(d => d.DeletedBy == DeletedBy.NotDeleted || d.DeletedBy == DeletedBy.Admin && (d.PosterId == currentUser.Id || currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.SuperAdmin))
                        .Where(WhereList(currentUser))
                       .Where(c => c.IsByAdmin);
        if (isRemoveAds)
            queryable = queryable.Where(x => x.PostType != PostType.Ads);

        var result = queryable.Select(MapList(currentUser));
        return result;
    }

    public IQueryable<PostDto> Recommended(User currentUser, bool isRemoveAds)
    {
        throw new NotImplementedException();
    }
    public bool Ratelimiter(int UserId, string Type)
    {
        var count = context.Post.Where(x => x.PosterId == UserId && x.CreatedDate.Date == DateTime.UtcNow.Date).Count();
         var daylimit=  context.RateLimit.Where(x => x.LimitType == Type).Select(x => x.LimitPrDay).FirstOrDefault();
        return count > daylimit;
    }
   
    public User GetUserByPosterId(int UserId)
    {
        return context.User.Where(x => x.Id == UserId).FirstOrDefault();
    }
    #region AbstractLogic
    public  /*static*/ Expression<Func<Post, bool>> WhereList(User currentUser)
    {

        currentUser.NotInterestedPostIds ??= new List<int>();

        return (post) => !post.Poster.Blocks.Any(c => c.BlockedId == currentUser.Id) && !post.Reports.Any(c => c.ReporterId == currentUser.Id) &&
                     !post.Poster.Blockers.Any(c => c.BlockerId == currentUser.Id) && post.IsCreatedInGroup == false &&
                     (!post.Poster.PrivateAccount || post.Poster.Followers.Any(x => x.FollowerId == currentUser.Id)) &&
                    !currentUser.NotInterestedPostIds.Any(x => post.Id == x) && post.Poster.IsDeletedAccount == false;

    }



    public  /*static*/ Expression<Func<Post, PostDto>> MapList(User currentUser)
    {


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
            PosterFollowerCount = post.Poster.Followers.Select(x=>x.Id).Count(),
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

    #endregion
}