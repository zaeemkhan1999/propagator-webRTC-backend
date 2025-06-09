namespace Apsy.App.Propagator.Application.Repositories;

public interface  IVerificationRequestRepository
 : IRepository<VerificationRequest>
{

    #region functions
    IQueryable<VerificationRequest> GetVerificationRequest();
    Task<VerificationRequest> GetVerificationRequestById(int requestId);
    IQueryable<User> GetUser();
#endregion
}
