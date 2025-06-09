namespace Apsy.App.Propagator.Application.Services;
public class LikeArticleCommentService : ServiceBase<LikeArticleComment, LikeArticleCommentInput>, ILikeArticleCommentService
{
    public LikeArticleCommentService(ILikeArticleCommentRepository repository) : base(repository)
    {
        this.repository = repository;
    }

    private readonly ILikeArticleCommentRepository repository;

    public override ResponseBase<LikeArticleComment> Add(LikeArticleCommentInput input)
    {
        var articleComment = repository.GetArticleComments((int)input.ArticleCommentId).FirstOrDefault();
        if (articleComment == null)
        {
            return ResponseStatus.NotFound;
        }
        
        if (!repository.Any((User a) => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        LikeArticleComment val = repository.GetArticleCommentsByCommentId((int)input.ArticleCommentId, (int)input.UserId).FirstOrDefault();

        if (val != null)
            return ResponseStatus.AlreadyExists;

        LikeArticleComment entity = new LikeArticleComment
        {
            UserId = (int)input.UserId,
            ArticleCommentId = (int)input.ArticleCommentId,
        };
        var newlikeComment = repository.Add(entity);
        articleComment.LikeCount++;
        repository.Update(articleComment);
        return newlikeComment;
    }

    public ResponseBase<LikeArticleComment> UnLikeArticleComment(LikeArticleCommentInput input)
    {
        LikeArticleComment likeArticleComment = repository.GetArticleCommentsByCommentId((int)input.ArticleCommentId, (int)input.UserId).FirstOrDefault();

        if (likeArticleComment == null)
            return ResponseStatus.NotFound;

        likeArticleComment.ArticleComment.LikeCount--;
        repository.Update(likeArticleComment.ArticleComment);
        return repository.Remove(likeArticleComment);
    }
}