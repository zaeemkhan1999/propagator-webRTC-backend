namespace Apsy.App.Propagator.Application.Services;

public class ArticleCommentService : ServiceBase<ArticleComment, ArticleCommentInput>, IArticleCommentService
{
    public ArticleCommentService(
        IArticleCommentRepository repository,
        IArticleRepository articleRepository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IEventStoreRepository eventStoreRepository,
        IInterestedUserService interestedUserService) : base(repository)
    {
        this.repository = repository;
        _articleRepository = articleRepository;
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
        this.interestedUserService = interestedUserService;
    }
    private readonly IArticleCommentRepository repository;
    private readonly IArticleRepository _articleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublisher _publisher;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    private readonly IInterestedUserService interestedUserService;
    public async Task<ResponseBase<ArticleComment>> AddArticleComment(ArticleCommentInput input, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.NotFound;

        if (input.UserId is null || input.UserId <= 0 || input.ArticleId is null)
        {
            return ResponseStatus.NotEnoghData;
        }

        if (input.CommentType == CommentType.Text && string.IsNullOrEmpty(input.Text))
            return CustomResponseStatus.TextIsRequired;

        if (input.CommentType != CommentType.Text && string.IsNullOrEmpty(input.ContentAddress))
            return CustomResponseStatus.ContentAddressIsRequired;

        var article = repository.Find<Article>((int)input.ArticleId);
        if (article == null) return ResponseStatus.NotFound;

        var isBlocked = repository.Any<BlockUser>(x => x.BlockerId == article.UserId && x.BlockedId == input.UserId);
        if (isBlocked) return CustomMessagingResponseStatus.CanNotCommentToBlocker;

        var comment = input.Adapt<ArticleComment>();
        var commentResult = repository.Add(comment);
        article.ArticleCommentsCount = await repository.Where(d => d.ArticleId == input.ArticleId && d.DeletedBy == DeletedBy.NotDeleted).CountAsync();

        await _articleRepository.UpdateArticlesEngagement(article);
        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            //settings.TotalArticleCommentsCount;
            settings.TotalArticleCommentsCount = await repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            repository.Update(settings);
        }

        string[] usernames = input.Text.GetUsernames().Distinct().ToArray();
        var users = repository.
            Where<User>(x => x.Id != currentUser.Id && usernames.Contains(x.Username))
            .Select(x => x.Id).ToList();

        foreach (var item in users)
        {
            try
            {
                await _publisher.Publish(new MentionedInArticleCommentEvent(commentResult, currentUser.Id, item));
            }
            catch
            {
            }
        }

        try
        {
            if (currentUser.Id != article.UserId)
            {
                var articelOwner = repository.Where<User>(c => c.Id == article.UserId).FirstOrDefault();
                if (articelOwner == null)
                    throw new Exception("poster is null");

                if (input.ParentId != null)
                {
                    var parentCommenter = repository.Where<Comment>(c => c.Id == input.ParentId).Select(c => new
                    {
                        c.UserId,
                        c.User.CommentNotification
                    }).FirstOrDefault();

                    if (parentCommenter != null && currentUser.Id != parentCommenter.UserId && parentCommenter.CommentNotification)
                    {
                        await _publisher.Publish(new AddReplyToArticleCommentEvent(commentResult.Id, currentUser.Id, parentCommenter.UserId));
                    }
                }
                if (articelOwner.CommentNotification)
                    await _publisher.Publish(new AddArticleCommentEvent(commentResult.Id, currentUser.Id, article.UserId));
            }
        }
        catch
        {
        }


        await interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = article.UserId, UserId = currentUser.Id, InterestedUserType = InterestedUserType.Article, ArticleId = article.Id });

        return new(commentResult);
    }

    public override ResponseBase<ArticleComment> Update(ArticleCommentInput input)
    {
        if (input.CommentType == CommentType.Text && string.IsNullOrEmpty(input.Text))
            return CustomResponseStatus.TextIsRequired;

        if (input.CommentType != CommentType.Text && string.IsNullOrEmpty(input.ContentAddress))
            return CustomResponseStatus.ContentAddressIsRequired;

        ArticleComment val = input.Adapt<ArticleComment>();
        ArticleComment val2 = repository.Find(val.Id);
        if (val2 == null)
        {
            return ResponseBase<ArticleComment>.Failure(ResponseStatus.NotFound);
        }

        if (input.Id != val2.UserId)
            return ResponseBase<ArticleComment>.Failure(ResponseStatus.NotFound);

        var articleId = val2.ArticleId;
        val2.Update<ArticleComment>(input);
        val2.ArticleId = articleId;
        val2.IsEdited = true;
        repository.Update(val2);
        return val2;
    }

    public async Task<ResponseStatus> SoftDeleteArticleComment(int entityId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        ArticleComment articleCommentFromDb = repository.Where(c => c.Id == entityId)
          .Include(x => x.Article)
          .ThenInclude(x => x.User)
          .FirstOrDefault();

        if (articleCommentFromDb == null)
        {
            return ResponseStatus.NotFound;
        }


        if (articleCommentFromDb.DeletedBy != DeletedBy.NotDeleted)
        {
            return ResponseStatus.AlreadyRemoved;
        }

        if (currentUser.UserTypes == UserTypes.User && articleCommentFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        // ArticleComment removedArticleComment = repository.Remove(articleCommentFromDb);
        articleCommentFromDb.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        ArticleComment removedArticleComment = repository.Update(articleCommentFromDb);

        if (removedArticleComment.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        articleCommentFromDb.Article.ArticleCommentsCount = await repository.Where(d => d.ArticleId == articleCommentFromDb.ArticleId && d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
        await _articleRepository.UpdateArticlesEngagement(articleCommentFromDb.Article);

        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalArticleCommentsCount = await repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            repository.Update(settings);
        }

        if (currentUser.UserTypes != UserTypes.User && articleCommentFromDb.UserId != currentUser?.Id)
        {
            articleCommentFromDb.RaiseEvent(ref _events, currentUser);
            _eventStoreRepository.SaveEvents(_events);
        }

        return ResponseStatus.Success;
    }

    public async Task<ResponseBase<bool>> SoftDeleteAll(List<int> ids, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var comments = repository.Where(r => ids.Contains(r.Id)).Include(c => c.Article).ThenInclude(d => d.User).AsQueryable();
        if (currentUser.UserTypes == UserTypes.User)
        {
            comments = comments.Where(r => r.UserId == currentUser.Id).AsQueryable();
        }
        var allComments = comments.ToList();
        var commentsCount = comments.Count();
        //repository.RemoveRange(comments);
        foreach (var item in allComments)
        {
            item.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;
        }
        repository.UpdateRange(allComments);

        var articles = comments.Select(c => c.Article).ToList();
        foreach (var item in articles)
        {
            item.ArticleCommentsCount = await repository.Where(d => d.ArticleId == item.Id && d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            await _articleRepository.UpdateArticlesEngagement(item);
        }

        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalArticleCommentsCount = await repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            repository.Update(settings);
        }

        foreach (var item in allComments)
        {
            item.RaiseEvent(ref _events, currentUser);
        }
        _eventStoreRepository.SaveEvents(_events);

        return true;
    }
    public async Task<ResponseStatus> UndoDeleteArticleComment(int entityId, User currentUser)
    {

        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        ArticleComment articleCommentFromDb = repository.UndoDeleteArticleComment(entityId);
        if (articleCommentFromDb == null)
        {
            return ResponseStatus.NotFound;
        }

        if (articleCommentFromDb.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        if (currentUser.UserTypes == UserTypes.User && articleCommentFromDb.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        articleCommentFromDb.DeletedBy = DeletedBy.NotDeleted;
        ArticleComment removedComment = repository.Update(articleCommentFromDb);

        articleCommentFromDb.Article.ArticleCommentsCount = await repository.Where(d => d.ArticleId == articleCommentFromDb.ArticleId && d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
        await _articleRepository.UpdateArticlesEngagement(articleCommentFromDb.Article);

        var settings = repository.GetDbSet<Settings>().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalArticleCommentsCount = await repository.Where(d => d.DeletedBy == DeletedBy.NotDeleted).CountAsync();
            repository.Update(settings);
        }

        return ResponseStatus.Success;
    }
}