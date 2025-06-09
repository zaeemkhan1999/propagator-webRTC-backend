using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class TagMutations
{
    [GraphQLName("tag_addViewToTags")]
    public async Task<ListResponseBase<Tag>> AddViewToTags(
       [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
       [Service] ITagService service,
       List<string> tagsText)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.AddViewToTags(tagsText, authentication.CurrentUser);
    }

    [GraphQLName("tag_sendnotif")]
    public async Task<ResponseBase> sendnotif(
    [Service] INotificationService service, int id)
    {
        await service.SendFirebaseCloudMessage(new Notification { Text = "hello", RecieverId = id });
        return ResponseBase.Success();
    }
}