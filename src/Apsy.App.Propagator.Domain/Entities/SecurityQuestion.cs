namespace Apsy.App.Propagator.Domain.Entities;

public class SecurityQuestion : EntityDef
{
    public string Question { get; set; }
    public ICollection<SecurityAnswer> SecurityAnswers { get; set; }

}
