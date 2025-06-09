namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class ResetPasswordRequestConfiguration : IEntityTypeConfiguration<ResetPasswordRequest>
{
    public void Configure(EntityTypeBuilder<ResetPasswordRequest> builder)
    {
        builder
            .Property(x => x.OtherFiles)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => (List<string>)JsonConvert.DeserializeObject<IList<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        builder.HasOne(x => x.User)
            .WithMany(x => x.ResetPasswordRequests)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}