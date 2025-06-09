namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IArticleCommentService : IServiceBase<ArticleComment, ArticleCommentInput>
{
    Task<ResponseBase<ArticleComment>> AddArticleComment(ArticleCommentInput input, User currentUser);
    Task<ResponseStatus> SoftDeleteArticleComment(int entityId,User currentUser);
    Task<ResponseBase<bool>> SoftDeleteAll(List<int> ids,User currentUser);
    Task<ResponseStatus> UndoDeleteArticleComment(int entityId, User currentUser);
}
