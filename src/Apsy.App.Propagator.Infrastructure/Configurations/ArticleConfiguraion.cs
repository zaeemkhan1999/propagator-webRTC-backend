using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Apsy.App.Propagator.Infrastructure.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder
           .Property(x => x.ArticleItems)
           .HasConversion(
               from => JsonConvert.SerializeObject(from, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
               to => (List<ArticleItem>)JsonConvert.DeserializeObject<IList<ArticleItem>>(to, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                   new ValueComparer<List<ArticleItem>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()));
    }
}
