namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleMostEngagedHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleMostEngagedHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }
    public override ListResponseBase<ArticleDto> Handle(object request,User currentUser)
    {
        
        currentUser.NotInterestedArticleIds ??= new List<int>();

        if ((GetArticleType)request == GetArticleType.MostEngaged)
        {
            var result = repository.MostEngaged(currentUser);

            return new(result);

        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }

}
