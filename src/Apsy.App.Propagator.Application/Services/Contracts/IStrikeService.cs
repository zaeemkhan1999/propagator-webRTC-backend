

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IStrikeService : IServiceBase<Strike, StrikeInput>
{
    Task<ResponseBase> RemoveStrike(int id,User currentUser);
}