namespace Apsy.App.Propagator.Application.Repositories;

public interface ICommentRepository : IRepository<Comment>
{

    public IQueryable<CommentDto> GetComment(int id,int UserId);
    public Comment GetComment(int id);
    public IQueryable<CommentDto> GetComments(User currentUser);
    public IQueryable<Comment> GetComments(List<int> ids, User currentUser);

    public int GetCommentCount(int id);
    public int GetCommentCount();

    IQueryable<Comment> GetCommentByPostId(int postId);
    
    int GetTotalCommentsCount();
    int GetPostTotalCommentsCount(int postId);
    bool isUserBlockForPostComment(int userId, int posterId);
}
