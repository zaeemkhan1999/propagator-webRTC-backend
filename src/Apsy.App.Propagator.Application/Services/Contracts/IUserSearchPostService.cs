namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserSearchPostService
 : IServiceBase<UserSearchPost, UserSearchPostInput>
{
    ResponseBase<UserSearchPost> DeleteSearchedPost(int userId, int postId);

}
