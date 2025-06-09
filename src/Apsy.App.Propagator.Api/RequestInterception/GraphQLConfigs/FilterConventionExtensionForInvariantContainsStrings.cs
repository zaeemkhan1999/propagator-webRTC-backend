using HotChocolate.Data.Filters.Expressions;

namespace Apsy.App.Propagator.Api.RequestInterception.GraphQLConfigs;

public class FilterConventionExtensionForInvariantContainsStrings : FilterConventionExtension
{
    protected override void Configure(IFilterConventionDescriptor descriptor)
    {
        descriptor.AddProviderExtension(new QueryableFilterProviderExtension(
            y => y.AddFieldHandler<QueryableStringInvariantContainsHandler>()));
    }
}