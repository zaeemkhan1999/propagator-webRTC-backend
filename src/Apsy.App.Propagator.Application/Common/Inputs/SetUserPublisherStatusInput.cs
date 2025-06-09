namespace Apsy.App.Propagator.Application.Common.Inputs;

public class SetUserPublisherStatusInput : InputDef
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public bool IsPublisher { get; set; }
}
