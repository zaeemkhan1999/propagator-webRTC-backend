using System.Collections;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Infrastructure.Repositories;


public class GroupRequestRepository: Repository<GroupRequest, DataReadContext>, IGroupRequestRepository
{
    public GroupRequestRepository(IDbContextFactory<DataReadContext> dbContextFactory) : base(dbContextFactory)
    {

    }

    public IQueryable<GroupRequestDto> GetPendingGroupRequests(int userId, int groupId)
    {
        var res = Context.GroupRequest.Where(x => x.GroupId == groupId && x.Status == "PENDING")
            .Select(x => new GroupRequestDto()
            {
                Id = x.Id,
                GroupId = x.GroupId,
                GroupAdminId = x.GroupAdminId,
                UserId = x.UserId,
                Status = x.Status,
                user = Context.User.Where(u => u.Id == userId).FirstOrDefault()
            }).AsQueryable();
        
            return res;
    }

    public IQueryable<GroupRequestDto> CheckJoinRequest(int userId, int groupId)
    {
        var res = Context.GroupRequest.Where(
            x => x.GroupId == groupId && x.UserId == userId)
            .Select(x => new GroupRequestDto()
            {
                Id = x.Id,
                GroupId = x.GroupId,
                GroupAdminId = x.GroupAdminId,
                UserId = x.UserId,
                Status = x.Status,
                user = Context.User.Where(u => u.Id == userId).FirstOrDefault()
            }).AsQueryable();
        return res;
    }
    public GroupRequest GetGroupRequestEntity(int userId, int groupId)
    {
        
        return Context.GroupRequest.Where(
            x => x.GroupId == groupId && x.UserId == userId)
            .Select(x => new GroupRequest()
            {
                Id = x.Id,
                GroupId = x.GroupId,
                GroupAdminId = x.GroupAdminId,
                UserId = x.UserId,
                Status = x.Status,
                user = Context.User.Where(u => u.Id == userId).FirstOrDefault()
            }).FirstOrDefault();
        
    }

}