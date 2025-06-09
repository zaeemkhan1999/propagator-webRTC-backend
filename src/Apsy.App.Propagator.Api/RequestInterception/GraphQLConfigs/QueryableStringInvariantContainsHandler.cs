using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;

namespace Apsy.App.Propagator.Api.RequestInterception.GraphQLConfigs;

public class QueryableStringInvariantContainsHandler : QueryableStringOperationHandler
{
    private static readonly MethodInfo _contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

    public QueryableStringInvariantContainsHandler(InputParser inputParser) : base(inputParser)
    {
    }

    protected override int Operation => DefaultFilterOperations.Contains;
    public override Expression HandleOperation(QueryableFilterContext context, IFilterOperationField field,
        IValueNode value, object parsedValue)
    {
        Expression property = context.GetInstance();
        if (parsedValue is string str)
        {
            var toLower = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);
            var finalExpression = Expression.Call(toLower, _contains, Expression.Constant(str.ToLower()));
            return finalExpression;

        }
        throw new InvalidOperationException();
    }
}
