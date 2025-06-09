namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class StoryDtoOpenSearch : DtoDef
    {
        public string textpositionx { get; set; } // Updated field
        public string textpositiony { get; set; } // Updated field
        public string textstyle { get; set; } // Updated field
        public int id { get; set; }
        public DateTime createddate { get; set; }
        public int userid { get; set; }
        public string contentaddress { get; set; }
        public StoryType storytype { get; set; }
        public string link { get; set; }
        public string text { get; set; }
        public bool likedbycurrentuser { get; set; }
        public bool seenbycurrentuser { get; set; }
        public HighLight highlight { get; set; }
        public int? highlightid { get; set; }
        public Post post { get; set; }
        public int? postid { get; set; }
        public Article article { get; set; }
        public int? articleid { get; set; }
        public int? duration { get; set; }
        public bool isliked { get; set; }
        public int likecount { get; set; }
        public int storyseenscount { get; set; }
        public int commentcount { get; set; }
        public DeletedBy deletedby { get; set; }
    }
}
