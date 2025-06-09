namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class UserFollowerConfiguration : IEntityTypeConfiguration<UserFollower>
{
    public void Configure(EntityTypeBuilder<UserFollower> builder)
    {
        builder.HasOne(a => a.Follower).WithMany(a => a.Followees).HasForeignKey(a => a.FollowerId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Followed).WithMany(a => a.Followers).HasForeignKey(a => a.FollowedId).OnDelete(DeleteBehavior.NoAction);
    }
}