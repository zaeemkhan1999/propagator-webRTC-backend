namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class EmailInput : InputDef
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string PlainTextContent { get; set; }
    public string HtmlContent { get; set; }
}
