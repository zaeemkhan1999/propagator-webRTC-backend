using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class CommentReadService : ServiceBase<Comment, CommentInput>,ICommentReadService
    {
        private readonly ICommentReadRepository repository;
        private readonly IPostReadRepository _postRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEventStoreReadRepository _eventStoreRepository;
        private List<BaseEvent> _events;
        public CommentReadService(
           ICommentReadRepository repository,
           IPostReadRepository postRepository,
           IHttpContextAccessor httpContextAccessor,
           IEventStoreReadRepository eventStoreRepository) : base(repository)
        {
            this.repository = repository;
            _postRepository = postRepository;
            _httpContextAccessor = httpContextAccessor;
            _eventStoreRepository = eventStoreRepository;
            _events = new List<BaseEvent>();
        }
        public SingleResponseBase<CommentDto> GetComment(int id, User currentUser)
        {

            var commentQueryable = repository.GetComment(id, currentUser.Id);
            if (commentQueryable.Any())
            {
                return new SingleResponseBase<CommentDto>(commentQueryable);
            }

            return ResponseStatus.NotFound;
        }

        public ListResponseBase<CommentDto> GetComments(bool loadDeleted, User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;
            var comments = repository.GetComments(currentUser);
            if (loadDeleted)
                comments = comments.IgnoreQueryFilters();
            return new(comments);
        }
    }
}
