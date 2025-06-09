using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class UserSearchGroupReadService : ServiceBase<UserSearchGroup, UserSearchGroupInput>, IUserSearchGroupReadService
    {
        private readonly IUserSearchGroupReadRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserSearchGroupReadService(IUserSearchGroupReadRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public SingleResponseBase<UserSearchGroupDto> GetUserSearchGroup(int id)
        {
            var queryable = repository.GetUserById(id).ProjectToType<UserSearchGroupDto>();
            if (queryable.Any())
            {
                return new SingleResponseBase<UserSearchGroupDto>(queryable);
            }

            return ResponseStatus.NotFound;
        }

        public ListResponseBase<UserSearchGroupDto> GetUserSearchGroups(User currentUser)
        {

            if (currentUser == null)
                return ResponseStatus.AuthenticationFailed;

            _ = TypeAdapterConfig<UserSearchGroup, UserSearchGroupDto>
              .NewConfig()
              .Map(dest => dest.MemberCount, src => src.Conversation.UserGroups.Count);

            var result = repository
                                .GetUserSearchGroup()
                                .Where(c => c.UserId == currentUser.Id)
                                .ProjectToType<UserSearchGroupDto>();

            return new(result);
        }

    }
}
