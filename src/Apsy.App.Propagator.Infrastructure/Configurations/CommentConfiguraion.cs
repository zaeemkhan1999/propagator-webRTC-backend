namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne(a => a.Mention).WithMany(a => a.CommentMentions).HasForeignKey(a => a.MentionId).OnDelete(DeleteBehavior.NoAction);
    }
}
