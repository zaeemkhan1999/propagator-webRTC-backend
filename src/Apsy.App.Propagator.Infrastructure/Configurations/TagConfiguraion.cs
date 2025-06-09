using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Infrastructure.Configurations;
using Tag = Tag;
public class TagConfiguration
 : IEntityTypeConfiguration<Tag>
{

    #region props
    #endregion
    #region functions
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
    }
    #endregion
}
