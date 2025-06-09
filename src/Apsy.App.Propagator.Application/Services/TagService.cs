using Tag = Apsy.App.Propagator.Domain.Entities.Tag;

namespace Apsy.App.Propagator.Application.Services;
public class TagService : ServiceBase<Tag, TagInput>, ITagService
{
    public TagService(ITagRepository repository, IHttpContextAccessor httpContextAccessor) : base(repository)
    {
        this.repository = repository;
    }

    private readonly ITagRepository repository;

    public ResponseBase<UserViewTag> AddViewToTag(string tagText,User currentUser)
    {
        
        if (currentUser == null)
            return ResponseStatus.NotFound;
 
        var tag = repository.GetTag().Where(c => c.Text == tagText).FirstOrDefault();
        if (tag == null)
            return ResponseStatus.NotFound;


        var userViewTag = repository.GetUserViewTag().Where(c => c.UserId == currentUser.Id && c.TagId == tag.Id).FirstOrDefault();
        if (userViewTag != null)
            return ResponseBase<UserViewTag>.Success(userViewTag);
 
        var userViewPost = new UserViewTag()
        {
            TagId = tag.Id,
            UserId = currentUser.Id,
        };

        tag.Hits++;
        UpdateSettings(1);
        repository.Update(tag);
        return ResponseBase<UserViewTag>.Success(repository.Add(userViewPost));
    }

    public async Task<ListResponseBase<Tag>> AddViewToTags(List<string> tagsText,User currentUser)
    {
        
        if (currentUser == null)
            return ResponseStatus.NotFound;

        var tags = await repository.GetTag().Where(c => tagsText.Contains(c.Text) && !c.UserViewTags.Any(x => x.UserId == currentUser.Id)).ToListAsync();

        if (!tags.Any())
            return ListResponseBase<Tag>.Success(tags.AsQueryable());

        var userViewTags = new List<UserViewTag>();
        int totalTagsView = 0;
        foreach (var item in tags)
        {
            userViewTags.Add(new UserViewTag
            {

                UserId = currentUser.Id,
                TagId = item.Id,
            });
            item.Hits++;
            totalTagsView++;
        }

        UpdateSettings(totalTagsView);
        await repository.AddRangeAsync(userViewTags);
        await repository.UpdateRangeAsync(tags);

        return ListResponseBase<Tag>.Success(tags.AsQueryable());
    }

    private void UpdateSettings(int totalTagsView)
    {
        var settings = repository.GetSettings().FirstOrDefault();
        if (settings != null)
        {
            settings.TotalTagsCount += totalTagsView;
            repository.Update(settings);
        }
    }
}