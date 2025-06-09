namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ISuspendService : IServiceBase<Suspend, SuspendInput>
{
    ResponseBase<User> UnSuspend(int userId);
}