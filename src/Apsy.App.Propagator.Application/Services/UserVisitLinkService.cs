namespace Apsy.App.Propagator.Application.Services;

public class UserVisitLinkService : ServiceBase<UserVisitLink, UserVisitLinkInput>, IUserVisitLinkService
{
    public UserVisitLinkService(
        IUserVisitLinkRepository repository,
        IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IUserVisitLinkRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;




    //get appsettings    setas adminStrator  delete ads     distinc she UserVisitLinkService

    public override ResponseBase<UserVisitLink> Add(UserVisitLinkInput input)
    {
        if (!repository.GetUser().Any(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        if (input.LinkType == LinkType.Article && input.ArticleId == null)
            return ResponseStatus.NotFound;
        if (input.LinkType == LinkType.Post && input.PostId == null)
            return ResponseStatus.NotFound;

        UserVisitLink userVisitLink = null;
        if (input.LinkType == LinkType.Post)
            userVisitLink = repository.GetUserVisitLink().Where(a => a.PostId == input.PostId && a.Link == input.Link && a.Text == input.Text && a.UserId == input.UserId).FirstOrDefault();
        if (input.LinkType == LinkType.Article)
            userVisitLink = repository.GetUserVisitLink().Where(a => a.ArticleId == input.ArticleId && a.Link == input.Link && a.Text == input.Text && a.UserId == input.UserId).FirstOrDefault();

        if (userVisitLink != null)
            return CustomResponseStatus.AlreadySaved;

        var newUserVisitLink = input.Adapt<UserVisitLink>();
        return repository.Add(newUserVisitLink);
    }

    public ResponseBase<UserVisitLink> DeleteUserVisitLink(int userId, string text, string link, int? postId, int? articleId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        UserVisitLink userVisitLink = null;
        if (postId != null)
            userVisitLink = repository.GetUserVisitLink().Where(a => a.PostId == postId && a.Link == link && a.Text == text && a.UserId == userId).FirstOrDefault();
        if (articleId != null)
            userVisitLink = repository.GetUserVisitLink().Where(a => a.ArticleId == articleId && a.Link == link && a.Text == text && a.UserId == userId).FirstOrDefault();
        if (userVisitLink == null)
            return ResponseStatus.NotFound;
        if (userVisitLink.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userVisitLink);
        return userVisitLink;
    }

    public async Task<ResponseBase<bool>> DeleteAllUserVisitLink()
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userVisitLinks = await repository
                                    .GetUserVisitLink().Where(a => a.UserId == currentUser.Id)
                                    .ToListAsync();

        repository.RemoveRange(userVisitLinks);
        return true;
    }

    private User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }
}