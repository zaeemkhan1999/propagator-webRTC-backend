namespace Apsy.App.Propagator.Application.Services;

public class InterestedUserService : ServiceBase<InterestedUser, InterestedUserInput>, IInterestedUserService
{
    public InterestedUserService(IInterestedUserRepository repository) : base(repository)
    {
        this.repository = repository;
    }
    private readonly IInterestedUserRepository repository;
    public async Task<ResponseBase<InterestedUser>> AddInterestedUser(InterestedUserInput input)
    {

        if (await repository.AnyAsync(d => d.UserId == input.UserId && d.ArticleId == input.ArticleId))
            return ResponseStatus.AlreadyExists;

        var interestedUser = input.Adapt<InterestedUser>();

        await repository.AddAsync(interestedUser);

        return ResponseBase<InterestedUser>.Success(interestedUser);
    }

    public async Task<ResponseStatus> DeleteInterestedUser(int entityId)
    {
        var query = repository.GetInterestedUsers(entityId);

        if (entityId == 0 || !query.Any())
            return ResponseStatus.NotFound;

        var interestedUser = await query.FirstOrDefaultAsync();

        await repository.RemoveAsync(interestedUser);

        return ResponseStatus.Success;
    }
}