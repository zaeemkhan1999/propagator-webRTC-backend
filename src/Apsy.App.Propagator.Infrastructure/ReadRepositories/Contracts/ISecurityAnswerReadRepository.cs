namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface ISecurityAnswerReadRepository : IRepository<SecurityAnswer>
{
    bool IsUserExist(int userId);
    IQueryable<SecurityAnswer> GetSecurityAnswer(int? id);
    IQueryable<SecurityAnswer> GetSecurityAnswerByUserId(int userId);
}
