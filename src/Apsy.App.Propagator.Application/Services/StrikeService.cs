namespace Apsy.App.Propagator.Application.Services;
public class StrikeService : ServiceBase<Strike, StrikeInput>, IStrikeService
{
    public StrikeService(
        IStrikeRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,
        IPublisher publisher) : base(repository)
    {
        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
        _publisher = publisher;
    }

    private readonly IStrikeRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    private readonly IPublisher _publisher;
    public override ResponseBase<Strike> Add(StrikeInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var post = new Post();
        var article = new Article();
        if (input.PostId != null)
            post = repository.GetPostById((int)input.PostId);
        if (input.ArticleId != null)
            article = repository.GetArticleById((int)input.ArticleId);

        var striketedUser = repository.GetUserById(input.UserId);
        if (striketedUser == null)
            return ResponseStatus.UserNotFound;

        if (striketedUser.UserTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        DateTime startDateTime = DateTime.UtcNow.Date; //Today; //Today at 00:00:00
        DateTime endDateTime = DateTime.UtcNow.AddDays(1).AddTicks(-1); //Today at 23:59:59

        var adminTodayLimitation =
                repository
                .GetAdminTodayLimitation().Where(a => a.UserId == currentUser.Id && a.CreatedDate > startDateTime && a.CreatedDate < endDateTime)
                .FirstOrDefault();

        if (currentUser.UserTypes == UserTypes.Admin && adminTodayLimitation != null && adminTodayLimitation.StrikeCount >= 20)
            return CustomResponseStatus.LimitTheNumberOfStrike;

        if (adminTodayLimitation == null)
        {

            var newAdminTodayLimitation = new AdminTodayLimitation()
            {
                UserId = currentUser.Id,
                StrikeCount = 1,
            };
            repository.Add(newAdminTodayLimitation);
        }

        if (adminTodayLimitation != null && adminTodayLimitation.StrikeCount < 20)
        {
            adminTodayLimitation.StrikeCount++;
            repository.Update(adminTodayLimitation);
        }

        var result = base.Add(input);

        var userStrikeCount = repository.GetStrikes().Where(c => c.UserId == input.UserId).Count();
        if (!striketedUser.IsSuspended && userStrikeCount % 3 == 0)
        {
            striketedUser.IsSuspended = true;
            striketedUser.SuspensionLiftingDate = DateTime.UtcNow.AddDays(1);
            repository.Update(striketedUser);

            repository.Add(new Suspend()
            {
                DayCount = 1,
                SuspendType = SuspendType.After3Strike,
                UserId = input.UserId,
                SuspensionLiftingDate = DateTime.UtcNow.AddDays(1),
            });
        }

        var userFromDb = repository.GetUserById(input.UserId);
        result.Result.User = userFromDb;
        result.Result.Post = post;
        result.Result.Article = article;
        result.Result.RaiseEvent(ref _events, currentUser,true);
        _eventStoreRepository.SaveEvents(_events);
        try
        {
            _publisher.Publish(new StrikeEvent(input.PostId,input.ArticleId, currentUser.Id, input.PostId is not null ? post.PosterId:article.UserId  )).GetAwaiter().GetResult();
        }
        catch
        {
        }
        return result;
    }


    public async Task<ResponseBase> RemoveStrike(int id,User currentUser)
    {
        
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var strike =await repository.GetStrikes().Where(d=>d.Id==id).Include(d=>d.Post).Include(d=>d.Article).FirstOrDefaultAsync();
        if (strike == null)
            return ResponseStatus.NotFound;

      //  var striketedUser =await repository.FindAsync<User>(strike.UserId);
        var striketedUser =   repository.GetUserById(strike.UserId);
        if (striketedUser == null)
            return ResponseStatus.UserNotFound;

        if (striketedUser.UserTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        DateTime startDateTime = DateTime.UtcNow.Date; //Today at 00:00:00
        DateTime endDateTime = DateTime.UtcNow.AddDays(1).AddTicks(-1); //Today at 23:59:59

        var adminTodayLimitation =
            repository
                .GetAdminTodayLimitation().Where(a => a.UserId == currentUser.Id && a.CreatedDate > startDateTime && a.CreatedDate < endDateTime)
                .FirstOrDefault();

        if (currentUser.UserTypes == UserTypes.Admin && adminTodayLimitation != null && adminTodayLimitation.StrikeCount >= 20)
            return CustomResponseStatus.LimitTheNumberOfStrike;


        if (adminTodayLimitation != null && adminTodayLimitation.StrikeCount < 20)
        {
            adminTodayLimitation.StrikeCount++;
            repository.Update(adminTodayLimitation);
        }

        var userStrikeCount =await repository.GetStrikes().Where(c => c.UserId == strike.UserId).CountAsync();
        if (striketedUser.IsSuspended && userStrikeCount %3==0)
        {
            striketedUser.IsSuspended = false;
            striketedUser.SuspensionLiftingDate = null;
            repository.Update(striketedUser);

            var supsend = await repository.GetSuspends().Where(d => d.UserId == striketedUser.Id && d.SuspendType == SuspendType.After3Strike).OrderByDescending(d=>d.Id).FirstOrDefaultAsync();

            await repository.RemoveAsync(supsend);
        }

        strike.RaiseEvent(ref _events, currentUser, false);
        _eventStoreRepository.SaveEvents(_events);

        await repository.RemoveAsync(strike);
        try
        {
            //_publisher.Publish(new UnstrikeEvent(strike.Id, currentUser.Id)).GetAwaiter().GetResult();
        }
        catch
        {
        }

        return ResponseBase.Success();
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