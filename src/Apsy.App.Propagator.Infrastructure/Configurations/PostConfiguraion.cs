using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Apsy.App.Propagator.Infrastructure.Configurations;
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasOne(a => a.Poster).WithMany(a => a.Posts).HasForeignKey(a => a.PosterId).OnDelete(DeleteBehavior.NoAction);

        var valueComparer = new ValueComparer<List<string>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList());

        builder
            .Property(x => x.Tags)
            .HasConversion(
                from => JsonConvert.SerializeObject(from, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }),
                to => (List<string>)JsonConvert.DeserializeObject<IList<string>>(to, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }))
                .Metadata.SetValueComparer(valueComparer);

        builder
           .Property(x => x.PostItems)
           .HasConversion(
               from => JsonConvert.SerializeObject(from, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
               to => (List<PostItem>)JsonConvert.DeserializeObject<IList<PostItem>>(to, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                   new ValueComparer<List<PostItem>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));
    }
}