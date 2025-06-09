namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasOne(a => a.Sender).WithMany(a => a.SenderNotifications).HasForeignKey(a => a.SenderId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Reciever).WithMany(a => a.RecieverNotifications).HasForeignKey(a => a.RecieverId).OnDelete(DeleteBehavior.NoAction);
    }
}