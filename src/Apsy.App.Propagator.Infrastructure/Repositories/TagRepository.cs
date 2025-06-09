
using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Infrastructure.Repositories;
public class TagRepository
 : Repository<Tag, DataReadContext>, ITagRepository
{
    public TagRepository(IDbContextFactory<DataReadContext> dbContextFactory)
    : base(dbContextFactory)
    {
        context = dbContextFactory.CreateDbContext();
    }

    #region props
    private DataReadContext context;


    #endregion
    #region functions
    public IQueryable<Tag> GetTag()
    {
        var query = context.Tag.AsQueryable();
        return query;
    }
    public IQueryable<UserViewTag> GetUserViewTag()
    {
        var query = context.UserViewTag.AsQueryable();
        return query;
    }
    public IQueryable<Settings> GetSettings()
    {
        var query = context.Settings.AsQueryable();
        return query;
    }

    #endregion
    #region functions

    public IEnumerable<string> GetUniqueTagsForPost(List<string> tags)
    {
        return tags.Where(c => !context.Tag.Any(x => x.Text == c));
        
    }
    #endregion
}
