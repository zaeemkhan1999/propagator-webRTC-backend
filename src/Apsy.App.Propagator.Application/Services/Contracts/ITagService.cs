using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface ITagService : IServiceBase<Tag, TagInput>
{
    ResponseBase<UserViewTag> AddViewToTag(string tagText,User currentUser);
    Task<ListResponseBase<Tag>> AddViewToTags(List<string> tagsText, User currentUser);
}