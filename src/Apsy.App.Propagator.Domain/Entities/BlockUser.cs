using System.ComponentModel.DataAnnotations.Schema;

namespace Apsy.App.Propagator.Domain.Entities;
public class BlockUser : EntityDef
{

    [ForeignKey("Blocker")]
    public int BlockerId { get; set; }
    public User Blocker { get; set; }

    [ForeignKey("Blocked")]
    public int BlockedId { get; set; }
    public User Blocked { get; set; }

    public bool IsMutual { get; set; }
}