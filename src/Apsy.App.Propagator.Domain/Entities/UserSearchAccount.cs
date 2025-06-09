using System.ComponentModel.DataAnnotations.Schema;

namespace Apsy.App.Propagator.Domain.Entities;

public class UserSearchAccount : EntityDef
{
    [ForeignKey("Searcher")]
    public int SearcherId { get; set; }
    public User Searcher { get; set; }

    [ForeignKey("Searched")]
    public int SearchedId { get; set; }
    public User Searched { get; set; }
}