using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Queries;

[ExtendObjectType(typeof(Query))]
public class DiscountQueries
{
    [GraphQLName("discount_getDiscount")]
    public SingleResponseBase<Discount> GetDiscount(
                        [Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IDiscountReadService service,
                        int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        return service.GetDiscount(entityId);
    }

    [GraphQLName("discount_getDiscountes")]
    public ListResponseBase<Discount> GetDiscountes(
        [Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IDiscountReadService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin && currentUser.UserTypes != UserTypes.User)
            return ResponseStatus.NotAllowd;

        return service.GetDiscounts();
    }



}