

namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class SecurityAnswerReadRepository
 : Repository<SecurityAnswer, DataWriteContext>, ISecurityAnswerReadRepository
{
public SecurityAnswerReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory )
:base(dbContextFactory)
{
	context = dbContextFactory.CreateDbContext();
}

    #region props
    private DataWriteContext context;

    #endregion
    #region functions
    public bool IsUserExist(int userId)
    {
        var answer = context.SecurityAnswer.Any(d => d.UserId == userId);
        return answer;
    }
    public IQueryable<SecurityAnswer> GetSecurityAnswer(int? id)
    {
        var answer = context.SecurityAnswer.Where(x => x.Id == id);
        return answer;
    }
    public IQueryable<SecurityAnswer> GetSecurityAnswerByUserId(int userId)
    {
        var answer = context.SecurityAnswer.Where(x => x.UserId == userId);
        return answer;
    }
    #endregion
}
