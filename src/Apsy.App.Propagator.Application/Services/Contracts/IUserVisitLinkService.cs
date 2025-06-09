namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserVisitLinkService : IServiceBase<UserVisitLink, UserVisitLinkInput>
{
    ResponseBase<UserVisitLink> DeleteUserVisitLink(int userId, string text, string link, int? postId, int? articleId);
    Task<ResponseBase<bool>> DeleteAllUserVisitLink();
}