namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchAccountService : IServiceBase<UserSearchAccount, UserSearchAccountInput>
{
    ResponseBase<UserSearchAccount> DeleteSearchedAccount(int searcherId, int searchedId);
    Task<ResponseBase<bool>> DeleteAllSearchedAccount();
}