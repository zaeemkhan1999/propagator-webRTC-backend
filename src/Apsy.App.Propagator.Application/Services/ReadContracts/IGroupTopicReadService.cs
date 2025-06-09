namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IGroupTopicReadService : IServiceBase<GroupTopic, GroupTopicInput>
    {
        ListResponseBase<GroupTopicDto> GetGroupTopic();
        SingleResponseBase<GroupTopicDto> GetGroupTopic(int id);
    }
}
