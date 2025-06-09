using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.ReadContracts
{
    public interface IFollowerReadService : IServiceBase<UserFollower, FollowerInput>
    {
        ResponseBase<FollowInfoDto> GetUserFollowInfo(int otherUserId, User currentUser);
        ListResponseBase<UserFollower> GetMyFollowersFollowers(User currentUser);
        ListResponseBase<UserFollower> GetFollowers(int userId);
        ListResponseBase<UserFollower> GetFollowings(int userId);
    }
}
