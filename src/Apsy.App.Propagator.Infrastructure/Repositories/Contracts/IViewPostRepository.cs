namespace Apsy.App.Propagator.Infrastructure.Repositories;

public interface  IViewPostRepository
 : IRepository<UserViewPost>
{

#region functions
#endregion
    IQueryable<UserViewPost> GetPostViews();
}
