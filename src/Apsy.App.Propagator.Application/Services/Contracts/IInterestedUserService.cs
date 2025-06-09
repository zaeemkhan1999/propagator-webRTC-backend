namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IInterestedUserService : IServiceBase<InterestedUser, InterestedUserInput>
{
    Task<ResponseBase<InterestedUser>> AddInterestedUser(InterestedUserInput input);
    Task<ResponseStatus> DeleteInterestedUser(int entityId);
}
