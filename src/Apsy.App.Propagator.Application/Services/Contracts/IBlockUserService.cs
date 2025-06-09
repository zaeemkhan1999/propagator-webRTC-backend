namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IBlockUserService : IServiceBase<BlockUser, BlockUserInput>
{
    ResponseBase<BlockUser> AddBlocked(BlockUserInput input);
    ResponseBase UnBlock(BlockUserInput input);
}