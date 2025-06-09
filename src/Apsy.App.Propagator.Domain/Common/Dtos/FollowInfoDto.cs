using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class FollowInfoDto : DtoDef
    {
        public bool IsMyFollower { get; set; }
        public bool IsMyFollowing { get; set; }
        public UserFollower Follower { get; set; }
        public UserFollower Following { get; set; }
    }
}