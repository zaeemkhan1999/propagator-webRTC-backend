namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchTagService : IServiceBase<UserSearchTag, UserSearchTagInput>
{

    ResponseBase<UserSearchTag> DeleteSearchedTag(int userId, string tag);
}
