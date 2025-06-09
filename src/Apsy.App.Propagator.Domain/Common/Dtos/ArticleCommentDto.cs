using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ArticleCommentDto : DtoDef
    {
        public ArticleComment ArticleComment { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
        public bool HasChild { get; set; }
        public int ChildrenCount { get; set; }
    }
}
