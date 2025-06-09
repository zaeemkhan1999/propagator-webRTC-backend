namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class InterestedUserConfiguration : IEntityTypeConfiguration<InterestedUser>
{
    public void Configure(EntityTypeBuilder<InterestedUser> builder)
    {
        builder.HasOne(a => a.FollowerUser).WithMany(a => a.InterestedUsers).HasForeignKey(a => a.FollowersUserId).OnDelete(DeleteBehavior.NoAction);
    }
}