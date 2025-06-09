namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class DiscountMutations
{
    [GraphQLName("discount_createDiscount")]
    public async Task<ResponseBase<Discount>> Create(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IDiscountService service,
        DiscountInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.AddDiscount(input);
    }


    [GraphQLName("discount_removeDiscount")]
    public async Task<ResponseStatus> Remove(
                    [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                    [Service(ServiceKind.Default)] IDiscountService service,
                    int entityId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;


        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.DeleteDiscount(entityId);
    }

    [GraphQLName("discount_updateDiscount")]
    public async Task<ResponseBase<Discount>> Update(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IDiscountService service,
        DiscountInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        return await service.UpdateDiscount(input);
    }

}