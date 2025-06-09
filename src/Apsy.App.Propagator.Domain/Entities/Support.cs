namespace Apsy.App.Propagator.Domain.Entities;

public class Support : EntityDef
{
    public string Email { get; set; }
    public string Text { get; set; }
    public string Category { get; set; }

    public User User { get; set; }
    public int? UserId { get; set; }
}