

namespace Apsy.App.Propagator.Application.DessignPattern.Articles;

public class ArticleExploreHandler : AbstractHandler
{
    private readonly IArticleRepository repository;

    public ArticleExploreHandler(IArticleRepository repository, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        this.repository = repository;
    }

    public override ListResponseBase<ArticleDto> Handle(object request,User currentUser)
    {

        if ((GetArticleType)request == GetArticleType.Explore)
        {
            
            var result = repository.Explore(currentUser);

            return new(result);
        }
        else
        {
            return base.Handle(request,currentUser);
        }
    }
}