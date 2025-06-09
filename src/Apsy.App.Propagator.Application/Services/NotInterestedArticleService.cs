namespace Apsy.App.Propagator.Application.Services;

public class NotInterestedArticleService : ServiceBase<NotInterestedArticle, NotInterestedArticleInput>, INotInterestedArticleService
{
    public NotInterestedArticleService(INotInterestedArticleRepository repository, IArticleRepository articleRepository) : base(repository)
    {
        this.repository = repository;
        _articleRepository = articleRepository;
    }

    private readonly INotInterestedArticleRepository repository;
    private readonly IArticleRepository _articleRepository;
    
    
    public async Task<ResponseBase<NotInterestedArticle>> AddNotInterestedArticle(NotInterestedArticleInput input)
    {
        var article = await repository.FindAsync<Article>((int)input.ArticleId);
        if (article == null)
        {
            return ResponseStatus.NotFound;
        }

        var user = repository.Find<User>((int)input.UserId);
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        NotInterestedArticle notInterestedArticle = repository.Where((NotInterestedArticle a) => a.ArticleId == input.ArticleId && a.UserId == input.UserId).FirstOrDefault();

        if (notInterestedArticle != null)
            return ResponseStatus.AlreadyExists;

        var exist = repository.Any<NotInterestedArticle>(c => c.UserId == user.Id && c.ArticleId == input.ArticleId);

        if (!exist)
        {
            if (user.NotInterestedArticleIds == null)
                user.NotInterestedArticleIds = new List<int>();

            if (!user.NotInterestedArticleIds.Any(c => c == input.ArticleId))
                user.NotInterestedArticleIds.Add((int)input.ArticleId);
        }


        NotInterestedArticle entity = new NotInterestedArticle
        {
            UserId = (int)input.UserId,
            ArticleId = (int)input.ArticleId,
        };
        article.NotInterestedArticlesCount++;
        
        repository.Update(user);
        var result = repository.Add(entity);
        await _articleRepository.UpdateArticlesEngagement(article);
        return result;
    }
    
    public async Task<ResponseBase<NotInterestedArticle>> RemoveFromNotInterestedArticle(NotInterestedArticleInput input)
    {
        NotInterestedArticle notInterestedArticle = repository.Where((NotInterestedArticle a) => a.ArticleId == input.ArticleId && a.UserId == input.UserId)
           .Include(c => c.Article)
            .Include(c => c.User).FirstOrDefault();

        if (notInterestedArticle == null)
            return ResponseStatus.NotFound;

        var notInterestedArticleIds = notInterestedArticle.User.NotInterestedArticleIds;
        if (notInterestedArticleIds != null && notInterestedArticleIds.Any(c => c == input.ArticleId))
            notInterestedArticle.User.NotInterestedArticleIds.Remove((int)input.ArticleId);
        repository.Update(notInterestedArticle.User);
        var result = repository.Remove(notInterestedArticle);
        await _articleRepository.UpdateArticlesEngagement(notInterestedArticle.Article);

        return result;
    }
}