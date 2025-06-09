
using System.ComponentModel.DataAnnotations.Schema;

namespace Apsy.App.Propagator.Domain.Entities;
public class Link : EntityDef
{
    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? ArticleId { get; set; }

    public LinkType LinkType { get; set; }

    [NotMapped]
    public int? EntityId { get; set; }

    public string Text { get; set; }
    public string Url { get; set; }
}