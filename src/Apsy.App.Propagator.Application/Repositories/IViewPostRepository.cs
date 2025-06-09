namespace Apsy.App.Propagator.Application.Repositories;

public interface  IViewPostRepository
 : IRepository<UserViewPost>
{

#region functions
#endregion
    IQueryable<UserViewPost> GetPostViews();
}
