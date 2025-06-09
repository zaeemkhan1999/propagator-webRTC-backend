using Tag = Apsy.App.Propagator.Domain.Entities.Tag;
namespace Apsy.App.Propagator.Application.Services.Read;

public class DashboardReadService : ServiceBase<Post, PostInput>, IDashboardReadService
{
    public DashboardReadService(
            ICommentReadRepository commentRepository,
            IPostLikeReadRepository postLikeRepository,
            IArticleReadRepository articleRepository,
            IArticleCommentReadRepository articleCommentRepository,
            IArticleLikeReadRepository articleLikeRepository,
            IUserReadRepository userRepository,
            ITagReadRepository tagRepository,
            ISettingsReadRepository settingsRepository,
            ISavePostReadRepository savePostRepository,
            ISaveArticleReadRepository saveArticleRepository,
            IViewArticleReadRepository viewArticleRepository,
            IPostReadRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IEventStoreReadRepository eventStoreRepository) : base(repository)
    {
        this.repository = repository;
        _commentRepository = commentRepository;
        _postLikeRepository = postLikeRepository;
        _articleRepository = articleRepository;
        _articleCommentRepository = articleCommentRepository;
        _articleLikeRepository = articleLikeRepository;
        _userRepository = userRepository;
        _tagRepository = tagRepository;
        _settingsRepository = settingsRepository;
        _savePostRepository = savePostRepository;
        _viewArticleRepository = viewArticleRepository;
        _saveArticleRepository = saveArticleRepository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
    }
    private readonly ICommentReadRepository _commentRepository;
    private readonly IPostLikeReadRepository _postLikeRepository;
    private readonly IArticleReadRepository _articleRepository;
    private readonly IArticleCommentReadRepository _articleCommentRepository;
    private readonly IArticleLikeReadRepository _articleLikeRepository;
    private readonly IUserReadRepository _userRepository;
    private readonly ITagReadRepository _tagRepository;
    private readonly ISettingsReadRepository _settingsRepository;
    private readonly ISavePostReadRepository _savePostRepository;
    private readonly IViewArticleReadRepository _viewArticleRepository;
    private readonly ISaveArticleReadRepository _saveArticleRepository;
    private readonly IPostReadRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventStoreReadRepository _eventStoreRepository;
    private List<BaseEvent> _events;

    private double calculateChange(long previous, long current)
    {
        if (previous == 0)
        {
            if (current == 0)
                return 0;
            return current * 100;
        }
        if (current == 0)
            return -previous * 100;


        var change = current - previous;
        return (double)change / previous * 100;
    }

    public async Task<ResponseBase<GetDashboardInfoDto>> GetDashboardInfo(DateTime startDate, DateTime endDate)
    {
        var Posts = repository.GetAllPosts();
        var Comments = _commentRepository.GetAllComments();
        var PostLikes = _postLikeRepository.GetAllPostLikes();
        var Articles = _articleRepository.GetAllArticle();
        var ArticleComments = _articleCommentRepository.GetAllArticleComment();
        var ArticleLikes = _articleLikeRepository.GetAllArticleLike();
        var Users = _userRepository.GetAllUser();
        var Tags = _tagRepository.GetTag();
        var Settings = _settingsRepository.GetAllSettings();

        GetDashboardInfoDto dashboardInfoDto = new();
        dashboardInfoDto.NumberOfPosts = await Posts.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var postInStartDateCount = await Posts.Where(c => startDate == c.CreatedDate).CountAsync();
        var postInEndDateCount = await Posts.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfPostsRatePercent = calculateChange(postInStartDateCount, postInEndDateCount);



        dashboardInfoDto.NumberOfPostComments = await Comments.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var commentInStartDateCount = await Comments.Where(c => startDate == c.CreatedDate).CountAsync();
        var commentInEndDateCount = await Comments.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfPostCommentsRatePercent = calculateChange(commentInStartDateCount, commentInEndDateCount);


        dashboardInfoDto.NumberOfPostLikes = await PostLikes.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var postLikesInStartDateCount = await PostLikes.Where(c => startDate == c.CreatedDate).CountAsync();
        var postLikesInEndDateCount = await PostLikes.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfPostLikesRatePercent = calculateChange(postLikesInStartDateCount, postLikesInEndDateCount);

        dashboardInfoDto.NumberOfPostPromotions = await Posts.Where(c => c.IsPromote && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var postPromotionsInStartDateCount = await Posts.Where(c => c.IsPromote && startDate == c.CreatedDate).CountAsync();
        var postPromotionsInEndDateCount = await Posts.Where(c => c.IsPromote && endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfPostPromotionsRatePercent = calculateChange(postPromotionsInStartDateCount, postPromotionsInEndDateCount);



        dashboardInfoDto.NumberOfArticles = await Articles.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var articleInStartDateCount = await Articles.Where(c => startDate == c.CreatedDate).CountAsync();
        var articleInEndDateCount = await Articles.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfArticlesRatePercent = calculateChange(articleInStartDateCount, articleInEndDateCount);




        dashboardInfoDto.NumberOfArticleComments = await ArticleComments.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var articleCommentInStartDateCount = await ArticleComments.Where(c => startDate == c.CreatedDate).CountAsync();
        var articleCommentInEndDateCount = await ArticleComments.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfArticleCommentsRatePercent = calculateChange(articleCommentInStartDateCount, articleCommentInEndDateCount);




        dashboardInfoDto.NumberOfArticleLikes = await ArticleLikes.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var articleLikeInStartDateCount = await ArticleLikes.Where(c => startDate == c.CreatedDate).CountAsync();
        var articleLikeInEndDateCount = await ArticleLikes.Where(c => endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfArticleLikesRatePercent = calculateChange(articleLikeInStartDateCount, articleLikeInEndDateCount);



        dashboardInfoDto.NumberOfArticlePromotions = await Articles.Where(c => c.IsPromote && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var articlePromotionsInStartDateCount = await Articles.Where(c => c.IsPromote && startDate == c.CreatedDate).CountAsync();
        var articlePromotionsInEndDateCount = await Articles.Where(c => c.IsPromote && endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfPostPromotionsRatePercent = calculateChange(articlePromotionsInStartDateCount, articlePromotionsInEndDateCount);



        dashboardInfoDto.NumberOfTags = Tags.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).Count();
        var tagInStartDateCount = await Articles.Where(c => c.IsPromote && startDate == c.CreatedDate).CountAsync();
        var tagInEndDateCount = await Articles.Where(c => c.IsPromote && endDate == c.CreatedDate).CountAsync();
        dashboardInfoDto.NumberOfTagsRatePercent = calculateChange(tagInStartDateCount, tagInEndDateCount);



        var allUsersCount = await Users.Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var privateUsersCount = await Users.Where(c => c.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var publicUsersCount = await Users.Where(c => !c.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();

        var womanUsersCount = await Users.Where(c => c.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var manUsersCount = await Users.Where(c => c.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var ratherNotUserSayCount = await Users.Where(c => c.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var customUserCount = await Users.Where(c => c.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);


        var accountWith0To15AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 0 && DateTime.Today.Year - c.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith15To20AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 15 && DateTime.Today.Year - c.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith20To25AgCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 20 && DateTime.Today.Year - c.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith25To30AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 25 && DateTime.Today.Year - c.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith30To35AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 30 && DateTime.Today.Year - c.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith35To40AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 35 && DateTime.Today.Year - c.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith40To120AgeCount = await Users.Where(c => DateTime.Today.Year - c.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();




        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        var settings = Settings.FirstOrDefault();
        dashboardInfoDto.TotalViewsInSystem = settings;

        dashboardInfoDto.Top5Tag = new Top5Tags();
        dashboardInfoDto.Top5Tag.TotalTagsView = settings.TotalTagsViews;

        dashboardInfoDto.Top5Tag.TopTags = Tags
                                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate)
                                            .OrderByDescending(c => c.Hits)
                                            .Take(5)
                                            .Select(c => new TopTag
                                            {
                                                Tag = c.Text,
                                                ViewCount = c.Hits,
                                            }).ToList();

        dashboardInfoDto.Top5TagLikesCounts = Tags
                                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate)
                                            .OrderByDescending(c => c.LikesCount)
                                            .Take(5)
                                            .Select(c => new TopTagLikesCount
                                            {
                                                Tag = c.Text,
                                                LikeCount = c.LikesCount
                                            }).ToList();

        dashboardInfoDto.Top5TagUsesCounts = Tags
                                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate)
                                            .OrderByDescending(c => c.UsesCount)
                                            .Take(5)
                                            .Select(c => new TopTagUsesCount
                                            {
                                                Tag = c.Text,
                                                UsesCount = c.UsesCount
                                            }).Where(d => d.UsesCount > 0).ToList();
        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetPostLikesDashboardInfo(DateTime startDate, DateTime endDate, int postId)
    {
        var PostLikes = _postLikeRepository.GetAllPostLikes();
        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await PostLikes
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await PostLikes
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await PostLikes
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await PostLikes
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await PostLikes
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await PostLikes
            .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var customUserCount = await PostLikes
            .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();


        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith15To20AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith20To25AgCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith25To30AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith30To35AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith35To40AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith40To120AgeCount = await PostLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetPostsCommentsDashboardInfo(DateTime startDate, DateTime endDate, int postId)
    {
        var Comments = _commentRepository.GetAllComments();
        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await Comments
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await Comments
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await Comments
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var womanUsersCount = await Comments
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await Comments
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await Comments
            .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var customUserCount = await Comments
            .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith15To20AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith20To25AgCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith25To30AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith30To35AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith35To40AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate).CountAsync();
        var accountWith40To120AgeCount = await Comments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetPostsViewsDashboardInfo(DateTime startDate, DateTime endDate, int postId)
    {
        var UserViewPost = repository.GetAllUserViewPost();
        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await UserViewPost
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await UserViewPost
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await UserViewPost
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await UserViewPost
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await UserViewPost
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await UserViewPost
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await UserViewPost
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith15To20AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith20To25AgCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith25To30AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith30To35AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith35To40AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith40To120AgeCount = await UserViewPost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetPostsSavesDashboardInfo(DateTime startDate, DateTime endDate, int postId)
    {
        var SavePost = _savePostRepository.GetAllSavePost();


        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await SavePost
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await SavePost
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await SavePost
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await SavePost
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await SavePost
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await SavePost
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await SavePost
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith15To20AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith20To25AgCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith25To30AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith30To35AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith35To40AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();
        var accountWith40To120AgeCount = await SavePost.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.PostId == postId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }


    public async Task<ResponseBase<DashboardInfo>> GetArticlesLikesDashboardInfo(DateTime startDate, DateTime endDate, int articleId)
    {
        var ArticleLikes = _articleLikeRepository.GetAllArticleLike();

        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await ArticleLikes
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await ArticleLikes
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await ArticleLikes
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await ArticleLikes
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await ArticleLikes
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId && c.ArticleId == articleId && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await ArticleLikes
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await ArticleLikes
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith15To20AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith20To25AgCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith25To30AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith30To35AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith35To40AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith40To120AgeCount = await ArticleLikes.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId && c.ArticleId == articleId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetArticlesCommentsDashboardInfo(DateTime startDate, DateTime endDate, int articleId)
    {
        var ArticleComments = _articleCommentRepository.GetAllArticleComment();

        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await ArticleComments
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await ArticleComments
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await ArticleComments
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await ArticleComments
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await ArticleComments
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();


        var ratherNotUserSayCount = await ArticleComments
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await ArticleComments
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();
        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith15To20AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith20To25AgCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith25To30AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith30To35AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith35To40AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith40To120AgeCount = await ArticleComments.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetArticlesViewsDashboardInfo(DateTime startDate, DateTime endDate, int articleId)
    {
        var ViewArticle = _viewArticleRepository.GetAllViewArticle();
        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await ViewArticle
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await ViewArticle
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await ViewArticle
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await ViewArticle
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await ViewArticle
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var ratherNotUserSayCount = await ViewArticle
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await ViewArticle
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith15To20AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith20To25AgCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith25To30AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith30To35AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith35To40AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith40To120AgeCount = await ViewArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    public async Task<ResponseBase<DashboardInfo>> GetArticlesSavesDashboardInfo(DateTime startDate, DateTime endDate, int articleId)
    {
        var SaveArticle = _saveArticleRepository.GetAllSaveArticle();


        DashboardInfo dashboardInfoDto = new();

        var allUsersCount = await SaveArticle
                            .Where(c => startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var privateUsersCount = await SaveArticle
                            .Where(c => c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();
        var publicUsersCount = await SaveArticle
                            .Where(c => !c.User.PrivateAccount && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                            .Select(c => c.User)
                            .Distinct()
                            .CountAsync();

        var womanUsersCount = await SaveArticle
            .Where(c => c.User.Gender == Gender.Male && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();

        var manUsersCount = await SaveArticle
            .Where(c => c.User.Gender == Gender.Female && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
            .Select(c => c.User)
            .Distinct()
            .CountAsync();


        var ratherNotUserSayCount = await SaveArticle
                .Where(c => c.User.Gender == Gender.RatherNotSay && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        var customUserCount = await SaveArticle
                .Where(c => c.User.Gender == Gender.Custom && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId)
                .Select(c => c.User)
                .Distinct()
                .CountAsync();

        dashboardInfoDto.PrivateAccountPercent = CalcPercent(privateUsersCount, allUsersCount);
        dashboardInfoDto.PublicAccountPercent = CalcPercent(publicUsersCount, allUsersCount);

        dashboardInfoDto.WomanAccountPercent = CalcPercent(womanUsersCount, allUsersCount);
        dashboardInfoDto.ManAccountPercent = CalcPercent(manUsersCount, allUsersCount);
        dashboardInfoDto.RatherNotSayAccountPercent = CalcPercent(ratherNotUserSayCount, allUsersCount);
        dashboardInfoDto.CustomAccountPercent = CalcPercent(customUserCount, allUsersCount);

        var accountWith0To15AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 0 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 15 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith15To20AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 15 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 20 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith20To25AgCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 20 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 25 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith25To30AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 25 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 30 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith30To35AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 30 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 35 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith35To40AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 35 && DateTime.Today.Year - c.User.DateOfBirth.Year <= 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId).CountAsync();
        var accountWith40To120AgeCount = await SaveArticle.Where(c => DateTime.Today.Year - c.User.DateOfBirth.Year > 40 && startDate < c.CreatedDate && c.CreatedDate < endDate && c.ArticleId == articleId && c.ArticleId == articleId).CountAsync();

        dashboardInfoDto.AccountWith0To15AgePercent = CalcPercent(accountWith0To15AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith15To20AgePercent = CalcPercent(accountWith15To20AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith20To25AgePercent = CalcPercent(accountWith20To25AgCount, allUsersCount);
        dashboardInfoDto.AccountWith25To30AgePercent = CalcPercent(accountWith25To30AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith30To35AgePercent = CalcPercent(accountWith30To35AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith35To40AgePercent = CalcPercent(accountWith35To40AgeCount, allUsersCount);
        dashboardInfoDto.AccountWith40To120AgePercent = CalcPercent(accountWith40To120AgeCount, allUsersCount);

        return dashboardInfoDto;
    }

    private int GetAge(DateTime dateOfBirth)
    {
        // Save today's date.
        var today = DateTime.Today;

        // Calculate the age.
        var age = today.Year - dateOfBirth.Year;
        return age;
    }

    private double CalcPercent(double current, double maximum)
    {
        if (maximum == 0)
            return 0;
        return current / maximum * 100;
    }
}