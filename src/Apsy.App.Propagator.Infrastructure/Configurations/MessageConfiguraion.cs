namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(a => a.ParentMessage).WithMany(a => a.ChildrenMessages).HasForeignKey(a => a.ParentMessageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
