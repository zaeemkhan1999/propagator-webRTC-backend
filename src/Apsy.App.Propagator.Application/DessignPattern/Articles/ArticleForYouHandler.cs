namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleForYouHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleForYouHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }

    public override ListResponseBase<ArticleDto> Handle(object request,User currentUser)
    {
        currentUser.NotInterestedArticleIds ??= new List<int>();

        if ((GetArticleType)request == GetArticleType.ForYou)
        {
            var result = repository.ForYou(currentUser);
            return new(result);
        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }

}
