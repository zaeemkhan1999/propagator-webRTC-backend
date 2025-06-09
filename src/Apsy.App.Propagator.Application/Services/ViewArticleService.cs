namespace Apsy.App.Propagator.Application.Services;

public class ViewArticleService
 : ServiceBase<ViewArticle,ViewArticleInput>,IViewArticleService
{
public ViewArticleService (IViewArticleRepository repository )
:base(repository)
{
		this.repository = repository;

}

#region props
	private IViewArticleRepository repository;

#endregion
#region functions
#endregion
}
