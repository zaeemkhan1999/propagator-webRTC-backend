namespace Apsy.App.Propagator.Application.Common
{
    public class StrikeDto : DtoDef
    {
        public int StrikeCount { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
    }
}