namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class VerificationRequestConfiguration : IEntityTypeConfiguration<VerificationRequest>
{

    public void Configure(EntityTypeBuilder<VerificationRequest> builder)
    {
        builder
            .Property(x => x.OtheFiles)
            .HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => (List<string>)JsonConvert.DeserializeObject<IList<string>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
    }
}