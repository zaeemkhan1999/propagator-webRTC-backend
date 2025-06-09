using Aps.CommonBack.Messaging.Generics.Responses;

namespace Apsy.App.Propagator.Application.Extensions;

public record CustomMessagingResponseStatus : MessagingResponseStatus
{

    public CustomMessagingResponseStatus(int code, string value) : base(code, value)
    {
    }

    public CustomMessagingResponseStatus(MessagingResponseStatus original) : base(original)
    {
    }

    public static ResponseStatus OnlySenderCanUpdateTheMessage = new CustomResponseStatus(200, nameof(OnlySenderCanUpdateTheMessage));
    public static ResponseStatus DeleteGroupNotAllowed = new CustomResponseStatus(201, nameof(DeleteGroupNotAllowed));
    public static ResponseStatus UserNotBelongToGroup = new CustomResponseStatus(202, nameof(UserNotBelongToGroup));
    public static ResponseStatus ConversationNotExist = new CustomResponseStatus(203, nameof(ConversationNotExist));
    public static ResponseStatus YouAreNotAMemberOfTheGroup = new CustomResponseStatus(204, nameof(YouAreNotAMemberOfTheGroup));
    public static ResponseStatus CouldNotRemoveAdminFromGroup = new CustomResponseStatus(205, nameof(CouldNotRemoveAdminFromGroup));
    public static ResponseStatus CanNotSendMessageToBlocker = new CustomResponseStatus(206, nameof(CanNotSendMessageToBlocker));
    public static ResponseStatus CanNotCommentToBlocker = new CustomResponseStatus(207, nameof(CanNotCommentToBlocker));
    public static ResponseStatus CanNotUpdateDeletedMessage = new CustomResponseStatus(208, nameof(CanNotUpdateDeletedMessage));
    public static ResponseStatus CanNotDeleteDeletedMessage = new CustomResponseStatus(209, nameof(CanNotDeleteDeletedMessage));
    public static ResponseStatus CanNotFollowBlocker = new CustomResponseStatus(210, nameof(CanNotFollowBlocker));
    public static ResponseStatus InvalidMessageType = new CustomResponseStatus(211, nameof(InvalidMessageType));
    public static ResponseStatus UserHasNotPermissionToPublishContent = new CustomResponseStatus(212, nameof(UserHasNotPermissionToPublishContent));
    public static ResponseStatus GroupTopicNotFound = new CustomResponseStatus(213, nameof(GroupTopicNotFound));
    public static ResponseStatus GroupNameAlreadyExists = new CustomResponseStatus(214, nameof(GroupNameAlreadyExists));
    public static ResponseStatus GroupLinkAlreadyExists = new CustomResponseStatus(215, nameof(GroupLinkAlreadyExists));
}