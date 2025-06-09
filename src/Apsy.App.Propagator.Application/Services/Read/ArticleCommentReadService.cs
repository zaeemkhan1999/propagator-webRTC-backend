using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class ArticleCommentReadService: ServiceBase<ArticleComment, ArticleCommentInput>, IArticleCommentReadService
    {
    
        private readonly IArticleCommentReadRepository repository;
        private readonly IArticleReadRepository _articleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        private readonly IEventStoreRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        private readonly IInterestedUserService interestedUserService;
        public ArticleCommentReadService(
        IArticleCommentReadRepository repository,
        IArticleReadRepository articleRepository,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IEventStoreRepository eventStoreRepository,
        IInterestedUserService interestedUserService) : base(repository)
        {
            this.repository = repository;
            _articleRepository = articleRepository;
            _httpContextAccessor = httpContextAccessor;
            _publisher = publisher;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
            this.interestedUserService = interestedUserService;
        }
        public SingleResponseBase<ArticleCommentDto> GetArticleComment(int id, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            var commentQueryable = repository.GetArticleComment(id, currentUser.Id);

            if (commentQueryable is not null)
            {
                return new SingleResponseBase<ArticleCommentDto>(commentQueryable);
            }

            return ResponseStatus.NotFound;
        }
        public ListResponseBase<ArticleCommentDto> GetArticleComments(bool loadDeleted, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            IQueryable<ArticleCommentDto> articleComments = repository.GetArticleComments(loadDeleted, currentUser);
            if (loadDeleted)
                articleComments = articleComments.IgnoreQueryFilters();
            return new(articleComments);
        }
    }
}
