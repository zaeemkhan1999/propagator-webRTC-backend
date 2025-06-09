namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class ArticleLikeQueries
 : QueryBase<ArticleLike, ArticleLikeInput, IArticleLikeReadService, User>
{
}
