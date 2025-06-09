namespace Apsy.App.Propagator.Application.Common.Inputs;

public class EmailInput : InputDef
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string PlainTextContent { get; set; }
    public string HtmlContent { get; set; }
}
