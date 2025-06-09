

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchGroupService : IServiceBase<UserSearchGroup, UserSearchGroupInput>
{
    ResponseBase<UserSearchGroup> DeleteSearchedGroup(int userId, int conversationId,User currentUser);
}