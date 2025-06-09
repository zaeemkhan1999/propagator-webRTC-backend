namespace Apsy.App.Propagator.Application.Services;

public class UserSearchGroupService
 : ServiceBase<UserSearchGroup, UserSearchGroupInput>, IUserSearchGroupService
{
    public UserSearchGroupService(IUserSearchGroupRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    private readonly IUserSearchGroupRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public override ResponseBase<UserSearchGroup> Add(UserSearchGroupInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (!repository.GetUser().Any(a => a.Id == input.UserId))
        {
            return ResponseStatus.UserNotFound;
        }

        var conversation = repository.FindConversation((int)input.ConversationId);
        if (conversation is null)
        {
            return ResponseStatus.UserNotFound;
        }

        if (!conversation.IsGroup)
            return ResponseStatus.NotAllowd;

        if (conversation.IsGroup && conversation.IsPrivate)
        {
            var isMemberOfGroup = repository.GetUserSearchGroup().Any(c => c.UserId == currentUser.Id && c.ConversationId == (int)input.ConversationId);
            if (!isMemberOfGroup)
            {
                return CustomMessagingResponseStatus.YouAreNotAMemberOfTheGroup;
            }
        }

        var userSearchGroup = repository.GetUserSearchGroup().Where(a => a.ConversationId == input.ConversationId && a.UserId == input.UserId).FirstOrDefault();
        if (userSearchGroup != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newUserSearchGroup = new UserSearchGroup { UserId = (int)input.UserId, ConversationId = (int)input.ConversationId };
        return repository.Add(newUserSearchGroup);
    }

    public ResponseBase<UserSearchGroup> DeleteSearchedGroup(int userId, int conversationId, User currentUser)
    {
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var userSearchedConversation = repository
                                    .GetUserSearchGroup().Where(a => a.ConversationId == conversationId && a.UserId == userId)
                                    .FirstOrDefault();

        if (userSearchedConversation == null)
            return ResponseStatus.NotFound;

        if (userSearchedConversation.UserId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        repository.Remove(userSearchedConversation);

        return userSearchedConversation;
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
