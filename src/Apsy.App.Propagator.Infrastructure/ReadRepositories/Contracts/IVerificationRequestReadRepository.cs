namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IVerificationRequestReadRepository
 : IRepository<VerificationRequest>
{

    #region functions
    IQueryable<VerificationRequest> GetVerificationRequest();
    Task<VerificationRequest> GetVerificationRequestById(int requestId);
    IQueryable<User> GetUser();
#endregion
}
