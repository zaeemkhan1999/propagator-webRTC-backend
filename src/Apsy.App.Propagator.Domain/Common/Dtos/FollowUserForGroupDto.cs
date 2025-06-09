namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class FollowUserForGroupDto : DtoDef
    {
        public User User { get; set; }
        public bool IsMemberOfGroup { get; set; }

    }
}
