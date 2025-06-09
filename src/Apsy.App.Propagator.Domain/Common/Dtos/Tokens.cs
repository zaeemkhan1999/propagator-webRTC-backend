namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class Tokens
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public DateTime ValidTo { get; set; }

       public int UserId { get; set; }
    }
}
