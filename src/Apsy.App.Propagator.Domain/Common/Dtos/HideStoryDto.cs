namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class HideStoryDto : DtoDef
    {
        public User Follower { get; set; }
        public bool IsHided { get; set; }
    }
}
