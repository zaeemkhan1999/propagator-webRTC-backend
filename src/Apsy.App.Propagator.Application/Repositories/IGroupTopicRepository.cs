namespace Apsy.App.Propagator.Application.Repositories;

public interface IGroupTopicRepository : IRepository<GroupTopic>
{
    IQueryable<GroupTopic> GetGroupTopicById(int Id);
    IQueryable<GroupTopic> GetGroupTopicById(int Converastaionid,int GrouptopicId);
    IQueryable<GroupTopicDto> GetGroupTopic(int Id);
    IQueryable<GroupTopicDto> GetGroupTopic();
}

