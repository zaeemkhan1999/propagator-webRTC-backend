using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Application.Repositories;
//using Tag = Propagator.Common.Models.Entities.Tag;
public interface  ITagRepository
 : IRepository<Tag>
{

    #region functions

    IQueryable<Tag> GetTag();
    IQueryable<UserViewTag> GetUserViewTag();
    IQueryable<Settings> GetSettings();
    IEnumerable<string> GetUniqueTagsForPost(List<string> tags);

#endregion
}
