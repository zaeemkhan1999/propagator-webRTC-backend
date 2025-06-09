namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class StorySeenDto : DtoDef
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public Story Story { get; set; }
        public int StoryId { get; set; }
        public bool IsLiked { get; set; }
    }
}