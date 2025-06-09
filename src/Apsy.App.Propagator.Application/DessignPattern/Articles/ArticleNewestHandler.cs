namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleNewestHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleNewestHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }

    public override ListResponseBase<ArticleDto> Handle(object request,User currentUser)
    {
        currentUser.NotInterestedArticleIds ??= new List<int>();

        if ((GetArticleType)request == GetArticleType.Newest)
        {
            var result = repository.GetNewestArticles(currentUser);

            return new(result);

        }
        else
        {
            return base.Handle(request, currentUser);
        }
    }

}
