namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class ArticleCommentConfiguration
 : IEntityTypeConfiguration<ArticleComment>
{

    #region props
    #endregion
    #region functions
    public void Configure(EntityTypeBuilder<ArticleComment> builder)
    {
        builder.HasOne(a => a.Mention).WithMany(a => a.ArticleCommentMentions).HasForeignKey(a => a.MentionId).OnDelete(DeleteBehavior.NoAction);

    }
    #endregion
}
