using Aps.CommonBack.Base.Models.Dtos;

namespace Apsy.App.Propagator.Domain.Common.Dtos
{
    public class ArticleAdsDto : DtoDef
    {
        public ArticleAdsDto(Article article, string clientSecret)
        {
            Article = article;
            ClientSecret = clientSecret;
        }

        public Article Article { get; set; }
        public string ClientSecret { get; set; }
    }
}