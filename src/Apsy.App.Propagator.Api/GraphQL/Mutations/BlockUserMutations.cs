namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class BlockUserMutations
{
    [GraphQLName("blockUser_blockUser")]
    public ResponseBase<BlockUser> BlockUser(
                                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                 BlockUserInput input,
                                 [Service] IBlockUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        input.BlockerId = currentUser.Id;
        if (input.BlockedId is null)
            return ResponseBase<BlockUser>.Failure(ResponseStatus.NotEnoghData);

        return ResponseBase<BlockUser>.Success(service.AddBlocked(input));
    }

    [GraphQLName("block_unBlockUser")]
    public ResponseBase<ResponseStatus> UnBlockUser(
                                            [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                            BlockUserInput input,
                                            [Service] IBlockUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return ResponseBase<ResponseStatus>.Failure(authentication.Status);
        }

        User currentUser = authentication.CurrentUser;

        input.BlockerId = currentUser.Id;
        if (input.BlockedId is null)
            return ResponseBase<ResponseStatus>.Failure(ResponseStatus.NotEnoghData);

        return ResponseBase<ResponseStatus>.Success(service.UnBlock(input));
    }
}
