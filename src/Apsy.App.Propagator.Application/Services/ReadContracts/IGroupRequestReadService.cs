namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IGroupRequestReadService : IServiceBase<GroupRequest, GroupRequestInput>
    {
        ListResponseBase<GroupRequestDto> GetPendingGroupRequests(int userId, int groupId);

        SingleResponseBase<GroupRequestDto> CheckJoinRequest(int userId, int groupId);

        GroupRequest GetGroupRequestEntity(int userId, int groupId);
    }
}