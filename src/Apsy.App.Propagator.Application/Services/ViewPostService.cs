namespace Apsy.App.Propagator.Application.Services;

public class ViewPostService
 : ServiceBase<UserViewPost,ViewPostInput>,IViewPostService
{
public ViewPostService (IViewPostRepository repository )
:base(repository)
{
		this.repository = repository;

}

#region props
	private IViewPostRepository repository;

#endregion
#region functions
#endregion
}
