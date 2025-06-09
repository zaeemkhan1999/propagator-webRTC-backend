namespace Apsy.App.Propagator.Application.Services;

public class SaveArticleService
 : ServiceBase<SaveArticle,SaveArticleInput>,ISaveArticleService
{
public SaveArticleService (ISaveArticleRepository repository )
:base(repository)
{
		this.repository = repository;

}

#region props
	private ISaveArticleRepository repository;

#endregion
#region functions
#endregion
}
