namespace Apsy.App.Propagator.Domain.Common.Dtos.Dtos
{
    public class TaskResquestModel
    {
        public Result[] results { get; set; }
    }

    public class Result
    {
        public Postitem postItem { get; set; }
        public Results results { get; set; }
    }

    public class Postitem
    {
        public int Order { get; set; }
        public string ThumNail { get; set; }
        public string Content { get; set; }
        public int PostItemType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Results
    {
        public string video { get; set; }
        public Thumbnail[] thumbnail { get; set; }
    }

    public class Thumbnail
    {
        public string URL { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
        public int size { get; set; }
    }

}

