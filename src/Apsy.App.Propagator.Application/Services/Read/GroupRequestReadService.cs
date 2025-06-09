

using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services;


public class GroupRequestReadService : ServiceBase<GroupRequest, GroupRequestInput>, IGroupRequestReadService
    {
        public GroupRequestReadService(IGroupRequestReadRepository repository) : base(repository)
       {
            _repository = repository;

       }

    IGroupRequestReadRepository _repository;

       public ListResponseBase<GroupRequestDto> GetPendingGroupRequests(int userId, int groupId)
       {
       
          var result = _repository.GetPendingGroupRequests(userId,groupId);
          return new ListResponseBase<GroupRequestDto>(result);
            
       }

       public SingleResponseBase<GroupRequestDto> CheckJoinRequest(int userId,int groupId)
       {
          var result = _repository.CheckJoinRequest(userId, groupId);
          return new SingleResponseBase<GroupRequestDto>(result);
            

       }

       public GroupRequest GetGroupRequestEntity(int userId, int groupId)
       {
            var result = _repository.GetGroupRequestEntity(userId, groupId);
            return result;
            
       }
    }
  
