namespace Apsy.App.Propagator.Infrastructure.Repositories;

public class GroupTopicReadRepository : Repository<GroupTopic, DataWriteContext>, IGroupTopicReadRepository
{
    public GroupTopicReadRepository(IDbContextFactory<DataWriteContext> dbContextFactory) 
        : base(dbContextFactory)
    {
    }

    public IQueryable<GroupTopicDto> GetGroupTopic(int Id)
    {
        var res= Context.GroupTopic.Where(x => x.Id == Id).Select(
         x => new GroupTopicDto()
         {
             ConversationId = x.ConversationId,
             Conversation = x.Conversation,
             Messages = x.Messages,
             Title = x.Title,
         }).AsQueryable();
        return res;
    }

    public IQueryable<GroupTopicDto> GetGroupTopic()
    {
        return Context.GroupTopic.Select(
               x => new GroupTopicDto()
               {
                   ConversationId = x.ConversationId,
                   Conversation = x.Conversation,
                   Messages = x.Messages,
                   Title = x.Title,
               });
    }

    public IQueryable<GroupTopic> GetGroupTopicById(int Id)
    {
        return Context.GroupTopic.Where(x => x.Id == Id);
    }
    public IQueryable<GroupTopic> GetGroupTopicByConversationId(int Id)
    {
        return Context.GroupTopic.Where(x => x.ConversationId == Id);
    }

    public IQueryable<GroupTopic> GetGroupTopicById(int Converastaionid, int GrouptopicId)
    {
        return Context.GroupTopic.Where(x => x.Id == GrouptopicId && x.ConversationId == Converastaionid);
    }

}
