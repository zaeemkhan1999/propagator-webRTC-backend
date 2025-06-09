namespace Apsy.App.Propagator.Application.Services;

public class CommentService : ServiceBase<Comment, CommentInput>, ICommentService
{
    public CommentService(
            ICommentRepository repository,
            IPostRepository postRepository,
            IHttpContextAccessor httpContextAccessor,
            IEventStoreRepository eventStoreRepository) : base(repository)
    {
        this.repository = repository;
        _postRepository = postRepository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
    }
    private readonly ICommentRepository repository;
    private readonly IPostRepository _postRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    public override ResponseBase<Comment> Add(CommentInput input)
    {

        if (input.UserId is null || input.UserId <= 0 || input.PostId == 0)
        {
            return ResponseStatus.NotEnoghData;
        }

        if (input.CommentType == CommentType.Text && string.IsNullOrEmpty(input.Text))
            return CustomResponseStatus.TextIsRequired;

        if (input.CommentType != CommentType.Text && string.IsNullOrEmpty(input.ContentAddress))
            return CustomResponseStatus.ContentAddressIsRequired;

        var post = repository.Find<Post>(input.PostId);
        if (post == null) return ResponseStatus.NotFound;

        var isBlocked = repository.Any<BlockUser>(x => x.BlockerId == post.PosterId && x.BlockedId == input.UserId);
        if (isBlocked) return CustomMessagingResponseStatus.CanNotCommentToBlocker;

        var comment = input.Adapt<Comment>();
        var commentResult = repository.Add(comment);
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalPostCommentsCount = repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync().GetAwaiter().GetResult();
            repository.Update(settings);
        }

        return new(commentResult);
    }

    public ResponseBase<Comment> Update(CommentInput input, User currentUser)
    {
        if (input.CommentType == CommentType.Text && string.IsNullOrEmpty(input.Text))
            return CustomResponseStatus.TextIsRequired;

        if (input.CommentType != CommentType.Text && string.IsNullOrEmpty(input.ContentAddress))
            return CustomResponseStatus.ContentAddressIsRequired;

        Comment val = input.Adapt<Comment>();
        Comment val2 = repository.GetComment(val.Id);
        if (val2 == null)
        {
            return ResponseBase<Comment>.Failure(ResponseStatus.NotFound);
        }

        if (currentUser.Id != val2.UserId)
            return ResponseStatus.NotFound;

        var postId = val2.PostId;
        val2.Update<Comment>(input);
        val2.PostId = postId;
        val2.IsEdited = true;
        repository.Update(val2);
        return val2;
    }
    public async Task<ResponseStatus> DeleteComment(int entityId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        Comment commentFromDb = repository.GetComment(entityId);

        if (commentFromDb == null)
        {
            return ResponseStatus.NotFound;
        }

        if (commentFromDb.DeletedBy != DeletedBy.NotDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        if (currentUser.UserTypes == UserTypes.User && commentFromDb.UserId != currentUser.Id && commentFromDb.Post.PosterId != currentUser.Id)
            return ResponseStatus.NotAllowd;
        var replies = await repository.GetReplies(entityId);
        foreach (var reply in replies)
        {
            await DeleteComment(reply.Id, currentUser);
        }


        commentFromDb.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        Comment removedComment = repository.Update(commentFromDb);

        if (removedComment.DeletedBy == DeletedBy.NotDeleted)
        {
            return ResponseStatus.Failed;
        }
        commentFromDb.Post.CommentsCount = repository.GetCommentCount(commentFromDb.PostId);
        await _postRepository.UpdatePostsEngagement(commentFromDb.Post);

        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalPostCommentsCount = await repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            repository.Update(settings);
        }

        if (currentUser.UserTypes != UserTypes.User && commentFromDb.UserId != currentUser?.Id)
        {
            commentFromDb.RaiseEvent(ref _events, currentUser);
            _eventStoreRepository.SaveEvents(_events);
        }
        return ResponseStatus.Success;
    }
    public async Task<ResponseBase<bool>> SoftDeleteAll(List<int> ids, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var comments = repository.GetComments(ids, currentUser);

        if (currentUser.UserTypes == UserTypes.User)
        {
            comments = comments.Where(r => r.UserId == currentUser.Id).AsQueryable();
        }

        var allComments = comments.ToList();
        var commentsCount = allComments.Count();
        var posts = allComments.Select(c => c.Post).ToList();

        foreach (var item in allComments)
        {
            item.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        }
        repository.UpdateRange(allComments);

        foreach (var item in posts)
        {
            item.CommentsCount = repository.GetCommentCount(item.Id);
            await _postRepository.UpdatePostsEngagement(item);
        }
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalPostCommentsCount = repository.GetCommentCount();
            repository.Update(settings);
        }

        foreach (var item in allComments)
        {
            item.RaiseEvent(ref _events, currentUser);
        }
        _eventStoreRepository.SaveEvents(_events);

        return true;
    }

    public async Task<ResponseStatus> UndoDeleteComment(int entityId, User currentUser)
    {
        Comment commentFromDb = repository.GetComment(entityId);

        if (commentFromDb == null)
        {
            return ResponseStatus.NotFound;
        }

        if (commentFromDb.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        if (currentUser.UserTypes == UserTypes.User && commentFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        commentFromDb.DeletedBy = DeletedBy.NotDeleted;
        Comment removedComment = repository.Update(commentFromDb);

        commentFromDb.Post.CommentsCount = repository.GetCommentCount(commentFromDb.PostId);
        await _postRepository.UpdatePostsEngagement(commentFromDb.Post);
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalPostCommentsCount = repository.GetCommentCount(commentFromDb.PostId);
            repository.Update(settings);
        }

        return ResponseStatus.Success;
    }
}