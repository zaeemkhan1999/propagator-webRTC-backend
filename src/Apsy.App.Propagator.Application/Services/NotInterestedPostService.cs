namespace Apsy.App.Propagator.Application.Services;

public class NotInterestedPostService : ServiceBase<NotInterestedPost, NotInterestedPostInput>, INotInterestedPostService
{
    public NotInterestedPostService(
        INotInterestedPostRepository repository,
        IPostRepository postRepository,
        IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _postRepository = postRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly INotInterestedPostRepository repository;
    private readonly IPostRepository _postRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public async Task<ResponseBase<NotInterestedPost>> AddNotInterested(NotInterestedPostInput input)
    {
        var post = await repository.FindAsync<Post>((int)input.PostId);
        if (post == null)
        {
            return ResponseStatus.NotFound;
        }

        var user = repository.Find<User>((int)input.UserId);
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        NotInterestedPost notInterestedPost = repository.Where((NotInterestedPost a) => a.PostId == input.PostId && a.UserId == input.UserId).FirstOrDefault();

        if (notInterestedPost != null)
            return ResponseStatus.AlreadyExists;

        var exist = repository.Any<NotInterestedPost>(c => c.UserId == user.Id && c.PostId == input.PostId);

        if (!exist)
        {
            if (user.NotInterestedPostIds == null)
                user.NotInterestedPostIds = new List<int>();
            if (!user.NotInterestedPostIds.Any(c => c == input.PostId))
                user.NotInterestedPostIds.Add((int)input.PostId);
        }

        NotInterestedPost entity = new NotInterestedPost
        {
            UserId = (int)input.UserId,
            PostId = (int)input.PostId,
        };

        post.NotInterestedPostsCount++;
        repository.Update(user);
        var result = repository.Add(entity);
        await _postRepository.UpdatePostsEngagement(post);
        return result;
    }

    public async Task<ResponseBase<NotInterestedPost>> RemoveFromNotInterestedPost(NotInterestedPostInput input)
    {
        NotInterestedPost notInterestedPost = repository.Where((NotInterestedPost a) => a.PostId == input.PostId && a.UserId == input.UserId).Include(c => c.User).Include(c => c.Post).FirstOrDefault();
        if (notInterestedPost == null)
            return ResponseStatus.NotFound;
        
        if (notInterestedPost.Post.NotInterestedPostsCount > 0)
            notInterestedPost.Post.NotInterestedPostsCount--;
        
        var notInterestedPostIds = notInterestedPost.User.NotInterestedPostIds;
        if (notInterestedPostIds != null && notInterestedPostIds.Any(c => c == input.PostId))
            notInterestedPost.User.NotInterestedPostIds.Remove((int)input.PostId);
        repository.Update(notInterestedPost.User);
        var result = repository.Remove(notInterestedPost);
        await _postRepository.UpdatePostsEngagement(notInterestedPost.Post);
     
        return result;
    }

}