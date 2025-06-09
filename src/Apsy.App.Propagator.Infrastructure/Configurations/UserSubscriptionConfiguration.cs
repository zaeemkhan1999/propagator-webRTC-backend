namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UsersSubscription>
{
    public void Configure(EntityTypeBuilder<UsersSubscription> builder)
    {
        builder.HasOne(x => x.Payment)
            .WithOne(x => x.UsersSubscription)
            .HasForeignKey<UsersSubscription>(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}