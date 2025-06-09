using Aps.CommonBack.Base.Repositories;

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class LikeCommentRepository
 : Repository<LikeComment, DataReadContext>, ILikeCommentRepository
{
public LikeCommentRepository (IDbContextFactory<DataReadContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

#region props
	private DataReadContext context;



    #endregion
    #region functions
    public IQueryable<LikeComment> GetLikeByCommentId(int Id, int UserId)
    {
        return context.LikeComment.Where(x => x.CommentId == Id && x.UserId==UserId);
    }
    public int GetLikeComments(int Id)
    {
        return context.LikeComment.Count(x => x.CommentId == Id);
    }
    #endregion
}
