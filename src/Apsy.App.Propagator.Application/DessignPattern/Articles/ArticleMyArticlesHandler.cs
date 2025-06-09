namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleMyArticlesHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleMyArticlesHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }

    public override ListResponseBase<ArticleDto> Handle(object request,User currentUser)
    {
        
        currentUser.NotInterestedArticleIds ??= new List<int>();

        if ((GetArticleType)request == GetArticleType.MyArticles)
        {
            var result = repository.MyArticles(currentUser);

            return new(result);
        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }

}
