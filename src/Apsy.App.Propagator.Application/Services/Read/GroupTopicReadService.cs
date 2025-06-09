using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class GroupTopicReadService : ServiceBase<GroupTopic, GroupTopicInput>, IGroupTopicReadService
    {
        public GroupTopicReadService(
        IGroupTopicReadRepository repository,
       IHttpContextAccessor httpContextAccessor,
       IPublisher publisher) : base(repository)
        {
            _repository = repository;
        }
        IGroupTopicReadRepository _repository;

        public ListResponseBase<GroupTopicDto> GetGroupTopic()
        {
            var postQueryable = _repository.GetGroupTopic();
            if (postQueryable.Any())
            {
                return new ListResponseBase<GroupTopicDto>(postQueryable);
            }

            return ResponseStatus.NotFound;
        }

        public SingleResponseBase<GroupTopicDto> GetGroupTopic(int id)
        {


            var postQueryable = _repository.GetGroupTopic(id);
            if (postQueryable.Any())
            {
                return new SingleResponseBase<GroupTopicDto>(postQueryable);
            }
            return ResponseStatus.NotFound;
        }
    }
}
