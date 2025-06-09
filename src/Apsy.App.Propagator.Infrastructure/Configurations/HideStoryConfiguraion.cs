namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class HideStoryConfiguration : IEntityTypeConfiguration<HideStory>
{
    public void Configure(EntityTypeBuilder<HideStory> builder)
    {
        builder.HasOne(a => a.Hider).WithMany(a => a.HidedStory).HasForeignKey(a => a.HiderId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Hided).WithMany(a => a.HiderStory).HasForeignKey(a => a.HidedId).OnDelete(DeleteBehavior.NoAction);
    }
}
