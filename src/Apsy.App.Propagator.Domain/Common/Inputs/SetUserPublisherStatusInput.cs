namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SetUserPublisherStatusInput : InputDef
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public bool IsPublisher { get; set; }
}
