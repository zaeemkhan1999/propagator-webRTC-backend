namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class SingleUserDto : DtoDef
    {
        public int ContentCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollwingCount { get; set; }

        public bool IsFollowing {get; set; }
        
        public User user {get; set;}
    }
}