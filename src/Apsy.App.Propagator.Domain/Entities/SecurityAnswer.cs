namespace Apsy.App.Propagator.Domain.Entities;

public class SecurityAnswer : EntityDef
{
    public User User { get; set; }
    public int UserId { get; set; }
    public SecurityQuestion SecurityQuestion { get; set; }
    public int SecurityQuestionId { get; set; }
    public string Answer { get; set; }
}