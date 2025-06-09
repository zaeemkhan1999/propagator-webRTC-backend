

namespace Apsy.App.Propagator.Application.Services.Contracts;
public interface ICommentService : IServiceBase<Comment, CommentInput>
{
    Task<ResponseStatus> DeleteComment(int entityId, User currentUser);
    Task<ResponseStatus> UndoDeleteComment(int entityId, User currentUser);
    Task<ResponseBase<bool>> SoftDeleteAll(List<int> ids, User currentUser);
    ResponseBase<Comment> Update(CommentInput input, User currentUser);
}