namespace Apsy.App.Propagator.Application.Services;

public class SavePostService
 : ServiceBase<SavePost,SavePostInput>,ISavePostService
{
public SavePostService (ISavePostRepository repository )
:base(repository)
{
		this.repository = repository;

}

#region props
	private ISavePostRepository repository;

#endregion
#region functions
#endregion
}
