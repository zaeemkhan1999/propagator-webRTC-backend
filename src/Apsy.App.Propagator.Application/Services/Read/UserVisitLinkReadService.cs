using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class UserVisitLinkReadService : ServiceBase<UserVisitLink, UserVisitLinkInput>, IUserVisitLinkReadService
    {
        private readonly IUserVisitLinkReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserVisitLinkReadService(
       IUserVisitLinkReadRepository repository,
       IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public override ListResponseBase<UserVisitLink> Get(Expression<Func<UserVisitLink, bool>> predicate = null, bool checkDeleted = false)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            Expression<Func<UserVisitLink, bool>> predicate2 = c =>
                            c.LinkType == LinkType.Post ?
                            c.UserId == currentUser.Id && (c.Post.LastModifiedDate == null || c.Post.LastModifiedDate < c.CreatedDate)
                            :
                            c.UserId == currentUser.Id;

            var result = repository.GetUserVisitLink().Where(predicate2);

            return new(result);
        }
        public ListResponseBase<UserVisitLink> UserVisitLinks()
        {
            var query = repository.GetUserVisitLink();

            return ListResponseBase.Success(query);
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
}
