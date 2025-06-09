namespace Apsy.App.Propagator.Domain.Entities;

public class GroupRequest : EntityDef
{
    //public int GroupRequestId { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public int GroupAdminId { get; set; }
    public string Status { get; set; }

    public User user { get; set; }
    //public Conversation conversation { get; set; }
}