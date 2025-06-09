namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class BlockUserConfiguration : IEntityTypeConfiguration<BlockUser>
{
    public void Configure(EntityTypeBuilder<BlockUser> builder)
    {
        builder.HasOne(a => a.Blocker).WithMany(a => a.Blocks).HasForeignKey(a => a.BlockerId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Blocked).WithMany(a => a.Blockers).HasForeignKey(a => a.BlockedId).OnDelete(DeleteBehavior.NoAction);
    }
}