namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SecretConversationInput : InputDef
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string PublicKey { get; set; }
}