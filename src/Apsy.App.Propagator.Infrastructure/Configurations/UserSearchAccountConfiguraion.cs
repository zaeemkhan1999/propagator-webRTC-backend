namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class UserSearchAccountConfiguration : IEntityTypeConfiguration<UserSearchAccount>
{
    public void Configure(EntityTypeBuilder<UserSearchAccount> builder)
    {
        builder.HasOne(a => a.Searcher).WithMany(a => a.SearchedAccounts).HasForeignKey(a => a.SearcherId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(a => a.Searched).WithMany(a => a.SearcherAccounts).HasForeignKey(a => a.SearchedId).OnDelete(DeleteBehavior.NoAction);
    }
}