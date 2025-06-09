using Apsy.App.Propagator.Application.Services.ReadContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsy.App.Propagator.Application.Services.Read
{
    public class FollowerReadService : ServiceBase<UserFollower, FollowerInput>, IFollowerReadService
    {
        private readonly IFollowerRepository repository;
        private readonly IBlockUserRepository _blockUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPublisher _publisher;
        public FollowerReadService(
        IFollowerRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IBlockUserRepository blockUserRepository,
        IPublisher publisher) : base(repository)
        {
            this.repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _blockUserRepository = blockUserRepository;
            _publisher = publisher;
        }
        public ListResponseBase<UserFollower> GetFollowers(int userId)
        {
            var followers = repository.Where(c => c.FollowedId == userId)
              .Include(c => c.Follower)
              .Where(c => !c.Follower.Blocks.Any(b => b.BlockedId == userId))
              .GroupBy(c => c.FollowerId)
              .Select(g => g.First())
              .ToList();
            if (followers == null)
                return ResponseStatus.NotFound;
            var sample = followers.GroupBy(x => x.FollowerId).Select(x => x.FirstOrDefault()).AsQueryable();
            return new ListResponseBase<UserFollower>(sample);
        }
        public ListResponseBase<UserFollower> GetFollowings(int userId)
        {
            var followings = repository.Where(c => c.FollowerId == userId)
                .Include(c => c.Followed)
                .Where(c => !c.Followed.Blocks.Any(b => b.BlockedId == userId))
                .GroupBy(c => c.FollowedId)
                .Select(g => g.First())
                .ToList();
            if (followings.Count == 0)
                return ResponseStatus.NotFound;
            var sample = followings.GroupBy(x => x.FollowedId).Select(x => x.FirstOrDefault()).AsQueryable();
            return new ListResponseBase<UserFollower>(sample);
        }
        public ResponseBase<FollowInfoDto> GetUserFollowInfo(int otherUserId, User currentUser)
        {
            return repository.GetUserFollowInfo(otherUserId, currentUser);
        }
        public ListResponseBase<UserFollower> GetMyFollowersFollowers(User currentUser)
        {
            return new(repository.GetMyFollowersFollowers(currentUser));
        }
        public override ListResponseBase<UserFollower> Get(Expression<Func<UserFollower, bool>> predicate = null, bool checkDeleted = false)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return ResponseStatus.NotFound;

            var followerAndFollowingsList = repository.Where(c =>
                               currentUser.Id == c.FollowedId ?
                                      !c.Follower.Blocks.Any(x => x.BlockedId == currentUser.Id)
                                       :
                                   !c.Followed.Blocks.Any(x => x.BlockedId == currentUser.Id))
               .Include(c => c.Follower).ToList();

            var uniqueFollowerAndFollowings = followerAndFollowingsList.GroupBy(x => x.FollowerId).Select(x => x.FirstOrDefault()).AsQueryable();

            return new ListResponseBase<UserFollower>(uniqueFollowerAndFollowings);
        }
        public virtual User GetCurrentUser()
        {
            var User = _httpContextAccessor.HttpContext.User;
            if (!User.Identity.IsAuthenticated)
                return null;

            var userString = User.Claims.FirstOrDefault(c => c.Type == "user").Value;
            var user = JsonConvert.DeserializeObject<User>(userString);
            return user;
        }
    }
}
