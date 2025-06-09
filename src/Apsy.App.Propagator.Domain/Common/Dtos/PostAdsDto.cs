namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class PostAdsDto : DtoDef
    {
        public PostAdsDto(Post post, string clientSecret)
        {
            Post = post;
            ClientSecret = clientSecret;
        }

        public Post Post { get; set; }

        public string ClientSecret { get; set; }

    }
}
