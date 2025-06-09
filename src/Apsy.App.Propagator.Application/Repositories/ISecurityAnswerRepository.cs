namespace Apsy.App.Propagator.Application.Repositories;

public interface ISecurityAnswerRepository : IRepository<SecurityAnswer>
{
    bool IsUserExist(int userId);
    IQueryable<SecurityAnswer> GetSecurityAnswer(int? id);
    IQueryable<SecurityAnswer> GetSecurityAnswerByUserId(int userId);
}
