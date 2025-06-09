namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class PaymentConfiguration
 : IEntityTypeConfiguration<Payment>
{

    #region props
    #endregion
    #region functions
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(x => x.UsersSubscription)
            .WithOne(x => x.Payment)
            .HasForeignKey<Payment>(x => x.UsersSubscriptionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
    #endregion
}
