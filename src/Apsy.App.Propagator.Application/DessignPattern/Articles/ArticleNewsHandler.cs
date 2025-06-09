namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleNewsHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleNewsHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }

    public override ListResponseBase<ArticleDto> Handle(object request, User currentUser)
    {
        if ((GetArticleType)request == GetArticleType.News)
        {
            var result =repository.GetNewsArticles(currentUser);
            return new(result);
        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }

}
