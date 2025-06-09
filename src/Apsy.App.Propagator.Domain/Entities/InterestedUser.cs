

namespace Apsy.App.Propagator.Domain.Entities;

public class InterestedUser : EntityDef
{
    public User FollowerUser { get; set; }
    public int FollowersUserId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public InterestedUserType InterestedUserType { get; set; }

    public Post Post { get; set; }
    public int? PostId { get; set; }

    public Article Article { get; set; }
    public int? ArticleId { get; set; }

}