namespace Apsy.App.Propagator.Application.Common
{
    public class FollowUserForGroupDto : DtoDef
    {
        public User User { get; set; }
        public bool IsMemberOfGroup { get; set; }

    }
}
