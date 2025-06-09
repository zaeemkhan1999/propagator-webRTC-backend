using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Application.Services.Read;
public class LinkReadService : ServiceBase<Link, LinkInput>, ILinkReadService
{
    private readonly IConfiguration _configuration;
    private readonly IUserReadRepository _userRepository;

    public LinkReadService(ILinkReadRepository repository,
        IConfiguration configuration,
        IUserReadRepository userRepository)
        : base(repository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    public async Task<ListResponseBase<Link>> Get(string searchTerm, int userId)
    {
        var customLink = await GetCustomLink(searchTerm.ToLower(), userId);
        if (customLink != null)
        {
            var response = new List<Link>
            {
                customLink
            };

            return new ListResponseBase<Link>(response.AsQueryable());
        }

        return base.Get();
    }

    /// <summary>
    /// https://qa.propagator.ca/propagator/home/mypost?id=183
    /// https://qa.propagator.ca/propagator/profile/
    /// https://qa.propagator.ca/propagator/singlepage/?username=abbas93
    /// https://qa.propagator.ca/propagator/group/detail/?id=15
    /// https://qa.propagator.ca/propagator/home/myarticle/?id=10
    /// </summary>
    private async Task<Link> GetCustomLink(string searchTerm, int userId)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return null;
        }

        var link = new Link
        {
            Url = searchTerm,
            Text = searchTerm
        };

        var baseUrl = _configuration["BaseUrl"]
            .Replace("https://", string.Empty)
            .Replace("http://", string.Empty);

        if (!searchTerm.Contains(baseUrl))
        {
            return null;
        }

        var splitedSearchTerm = searchTerm
            .Replace("https://", string.Empty)
            .Replace("http://", string.Empty)
            .Split('/');

        //https://qa.propagator.ca/propagator/home/mypost?id=183
        if (searchTerm.Contains("mypost") &&
            int.TryParse(splitedSearchTerm.Single(x => x.Contains('?')).Split('=').Last(), out int postId))
        {
            link.PostId = postId;
            link.EntityId = postId;
            link.LinkType = LinkType.Post;
        }

        //https://qa.propagator.ca/propagator/profile/
        if (searchTerm.Contains("profile"))
        {
            link.EntityId = userId;
            link.LinkType = LinkType.Profile;
        }

        //https://qa.propagator.ca/propagator/singlepage/?username=abbas93
        if (searchTerm.Contains("singlepage"))
        {
            var username = splitedSearchTerm.Single(x => x.Contains('?')).Split('=').LastOrDefault() ?? string.Empty;

            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var user = await _userRepository
                .Where(x => x.Username.Equals(username))
                .SingleOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            link.EntityId = user.Id;
            link.LinkType = LinkType.Profile;
        }

        //https://qa.propagator.ca/propagator/group/detail/?id=15
        if (searchTerm.Contains("group") &&
            int.TryParse(splitedSearchTerm.Single(x => x.Contains('?')).Split('=').Last(), out int groupId))
        {
            link.EntityId = groupId;
            link.LinkType = LinkType.MessageGroup;
        }

        //https://qa.propagator.ca/propagator/home/myarticle/?id=10
        if (searchTerm.Contains("myarticle") &&
            int.TryParse(splitedSearchTerm.Single(x => x.Contains('?')).Split('=').Last(), out int articleId))
        {
            link.ArticleId = articleId;
            link.EntityId = articleId;
            link.LinkType = LinkType.Article;
        }

        return link;
    }
}