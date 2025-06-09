

namespace Apsy.App.Propagator.Domain.Entities
{
    public class UserVisitLink : UserKindDef<User>
    {
        public Post Post { get; set; }
        public int? PostId { get; set; }

        public Article Article { get; set; }
        public int? ArticleId { get; set; }

        public LinkType LinkType { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
    }
}