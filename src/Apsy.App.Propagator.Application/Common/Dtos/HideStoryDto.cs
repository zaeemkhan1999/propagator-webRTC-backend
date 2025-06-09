namespace Apsy.App.Propagator.Application.Common
{
    public class HideStoryDto : DtoDef
    {
        public User Follower { get; set; }
        public bool IsHided { get; set; }
    }
}
