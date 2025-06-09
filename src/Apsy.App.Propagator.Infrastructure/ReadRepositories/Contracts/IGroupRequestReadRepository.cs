namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IGroupRequestReadRepository : IRepository<GroupRequest>
{
    IQueryable<GroupRequestDto> GetPendingGroupRequests(int userId, int groupId);

    IQueryable<GroupRequestDto> CheckJoinRequest(int groupId, int userId);

    GroupRequest GetGroupRequestEntity(int userId, int groupId);

}