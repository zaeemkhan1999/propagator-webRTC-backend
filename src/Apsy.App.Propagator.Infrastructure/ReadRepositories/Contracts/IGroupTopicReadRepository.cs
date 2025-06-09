namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IGroupTopicReadRepository : IRepository<GroupTopic>
{
    IQueryable<GroupTopic> GetGroupTopicById(int Id);
    IQueryable<GroupTopic> GetGroupTopicById(int Converastaionid,int GrouptopicId);
    IQueryable<GroupTopicDto> GetGroupTopic(int Id);
    IQueryable<GroupTopic> GetGroupTopicByConversationId(int Id);
    
    IQueryable<GroupTopicDto> GetGroupTopic();
}

