using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class ArticleReadService: ServiceBase<Article, ArticleInput>, IArticalReadService
    {
        private readonly IArticleReadRepository repository;
        private readonly ISaveArticleReadRepository _saveArticleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IPublisher _publisher;
        private readonly IArticleLikeReadRepository _articleLikeRepository;
        private readonly IEventStoreReadRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        private readonly IInterestedUserService interestedUserService;
        public ArticleReadService(
        IArticleReadRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IPaymentService paymentService,
        IConfiguration configuration,
        IArticleLikeReadRepository articleLikeRepository,
        IPublisher publisher,
        IEventStoreReadRepository eventStoreRepository
, ISaveArticleReadRepository saveArticleRepository,
        IInterestedUserService interestedUserService) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
            _configuration = configuration;
            _publisher = publisher;
            _eventStoreRepository = eventStoreRepository;
            _saveArticleRepository = saveArticleRepository;
            _articleLikeRepository = articleLikeRepository;
            _events = new List<BaseEvent>();
            this.interestedUserService = interestedUserService;
        }
        public SingleResponseBase<ArticleDto> GetArticle(int id, int UserId)
        {
            var articleQueryable = repository.GetArticle(id, UserId);


            if (articleQueryable is not null)
            {
                return new SingleResponseBase<ArticleDto>((IQueryable<ArticleDto>)articleQueryable);
            }

            return ResponseStatus.NotFound;
        }

        public ListResponseBase<ArticleDto> GetArticles(User currentUser)
        {
            var articles = repository.GetArticles(currentUser);

            return new(articles);
        }
        public ListResponseBase<ArticleDto> GetTopArticles(DateTime from, int UserId)
        {
            var articles = repository.GetTopArticles(from, UserId);
            return new((IQueryable<ArticleDto>)articles);
        }

        public ListResponseBase<ArticleDto> GetFollowersArticles(int UserId)
        {
            var articles = repository.GetFollowersArticles(UserId);
            return new((IQueryable<ArticleDto>)articles);
        }
        public ListResponseBase<UserViewArticle> GetViews()
        {
            var views = repository.GetDbSet<UserViewArticle>();
            return ListResponseBase<UserViewArticle>.Success(views);
        }

        public ListResponseBase<SaveArticleDto> GetSavedArticles(User currentUser)
        {
            return new(_saveArticleRepository.GetSavedArticles(currentUser));
        }
    }
}
