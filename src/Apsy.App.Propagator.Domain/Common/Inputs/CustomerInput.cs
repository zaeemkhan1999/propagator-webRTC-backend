namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class CustomerInput : InputDef  /*CustomerInputDef*/
{

    public string Email { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long? Balance { get; set; }

    public Dictionary<string, string> Metadata { get; set; }
}
