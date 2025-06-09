using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Application.Common
{
    public class CommentDto : DtoDef
    {
        public Comment Comment { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
        public bool HasChild { get; set; }
        public int ChildrenCount { get; set; }
    }
}
