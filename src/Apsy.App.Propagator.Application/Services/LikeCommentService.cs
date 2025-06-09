namespace Apsy.App.Propagator.Application.Services;
public class LikeCommentService : ServiceBase<LikeComment, LikeCommentInput>, ILikeCommentService
{
    public LikeCommentService(ILikeCommentRepository repository, ICommentRepository commentrepository) : base(repository)
    {
        this.repository = repository;
        _commentrepository = commentrepository;  
    }

    private readonly ILikeCommentRepository repository;
    private readonly ICommentRepository _commentrepository;

    public override ResponseBase<LikeComment> Add(LikeCommentInput input)
    {

        var comment = _commentrepository.GetComment((int)input.CommentId);
        if (comment == null)
        {
            return ResponseStatus.NotFound;
        }

        if (!repository.Any((User a) => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        LikeComment likeComment = repository.GetLikeByCommentId((int)input.CommentId, (int)input.UserId).FirstOrDefault();

        if (likeComment != null)
            return ResponseStatus.AlreadyExists;

        LikeComment entity = new LikeComment
        {
            UserId = (int)input.UserId,
            CommentId = (int)input.CommentId,
        };
        var newlikeComment = repository.Add(entity);
        comment.LikeCount=repository.GetLikeComments((int)input.CommentId);
        repository.Update(comment);
        return newlikeComment;
    }

    public ResponseBase<LikeComment> UnlikeComment(LikeCommentInput input)
    {
        LikeComment likeComment = repository.GetLikeByCommentId((int)input.CommentId ,(int)input.UserId).Include(c => c.Comment).FirstOrDefault();

        if (likeComment == null)
            return ResponseStatus.NotFound;

        likeComment.Comment.LikeCount= repository.GetLikeComments((int)input.CommentId);
        repository.Update(likeComment.Comment);
        return repository.Remove(likeComment);
    }
}