namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(x => x.NotInterestedPostIds)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<int>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        builder
            .Property(x => x.NotInterestedArticleIds)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<int>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        builder.HasMany(x => x.ResetPasswordRequests)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}