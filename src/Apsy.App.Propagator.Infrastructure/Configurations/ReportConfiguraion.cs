namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class ReportUserConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasOne(a => a.Reporter).WithMany(a => a.Reports).HasForeignKey(a => a.ReporterId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Reported).WithMany(a => a.Reporters).HasForeignKey(a => a.ReportedId).OnDelete(DeleteBehavior.NoAction);
    }
}