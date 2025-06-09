namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class StoryUserDto : DtoDef
    {
        public List<Story> Stories { get; set; }
        public bool HasNotSeenedStory { get; set; }
        public User StoryOwner { get; set; }
        public int StoryCount { get; set; }
        public int SeenedStoryCount { get; set; }
    }
}