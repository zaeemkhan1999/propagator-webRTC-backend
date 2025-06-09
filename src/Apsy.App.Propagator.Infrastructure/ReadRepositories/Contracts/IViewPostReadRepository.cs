namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface IViewPostReadRepository
 : IRepository<UserViewPost>
{

#region functions
#endregion
    IQueryable<UserViewPost> GetPostViews();
}
