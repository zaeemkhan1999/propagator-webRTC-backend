using Apsy.App.Propagator.Domain.Common.Dtos.Dtos;
using Apsy.App.Propagator.Infrastructure.Redis;
using Hangfire;
using Newtonsoft.Json.Linq;
using Propagator.Common.Services.Contracts;
using Tag = Apsy.App.Propagator.Domain.Entities.Tag;
using User = Apsy.App.Propagator.Domain.Entities.User;

namespace Apsy.App.Propagator.Application.Services;
public class PostService : ServiceBase<Post, PostInput>, IPostService
{
    private readonly IRedisCacheService _redisCache;
    private readonly IPostRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    private readonly IPublisher _publisher;
    private readonly IEventStoreRepository _eventStoreRepository;
    private List<BaseEvent> _events;
    private readonly IApplicationLogRepository _logs;
    private readonly IAdReadRepository _adRepository;
    private readonly IInterestedUserService _interestedUserService;
    private readonly ITagRepository _tagRepository;
    private readonly ISettingsRepository _settingRepository;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IViewPostRepository _viewPostRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IPlaceRepository _placeRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IUserDiscountRepository _userDiscountRepository;
    private readonly ILinkRepository _linkRepository;
    private readonly CompressionApiClient _compressionApi;
    private readonly IUsersSubscriptionService _usersSubscriptionService;
    
    public PostService(IPostRepository repository,
        IRedisCacheService redisCache,
        IHttpContextAccessor httpContextAccessor,
        IPaymentService paymentService,
        IConfiguration configuration,
        IPublisher publisher,
        CompressionApiClient compressionApiClient,
        IEventStoreRepository eventStoreRepository,
        IAdReadRepository adRepository,
        IApplicationLogRepository logs,
        IInterestedUserService interestedUserService,
        System.Net.Http.IHttpClientFactory httpClientFactory,
        ITagRepository tagRepository,
        ISettingsRepository settingRepository,
        IPostLikeRepository postLikeRepository,
        IViewPostRepository viewPostRepository,
        ICommentRepository commentRepository,
        IPlaceRepository placeRepository,
        IDiscountRepository discountRepository,
        ILinkRepository linkRepository,
        IUserDiscountRepository userDiscountRepository,
        IUsersSubscriptionService usersSubscriptionService,
        IUserRepository userRepository)
        : base(repository)
    {
        _redisCache = redisCache;
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _paymentService = paymentService;
        _configuration = configuration;
        _publisher = publisher;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
        _adRepository = adRepository;
        _interestedUserService = interestedUserService;
        _tagRepository = tagRepository;
        _settingRepository = settingRepository;
        _postLikeRepository = postLikeRepository;
        _viewPostRepository = viewPostRepository;
        _commentRepository = commentRepository;
        _placeRepository = placeRepository;
        _discountRepository = discountRepository;
        _userDiscountRepository = userDiscountRepository;
        _userRepository = userRepository;
        _linkRepository = linkRepository;
        _compressionApi = compressionApiClient;
        _logs = logs;
        _usersSubscriptionService = usersSubscriptionService;
    }
    public async Task<ResponseBase<List<PostWatchHistory>>> AddWatchHistory(int[] ids)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            throw new Exception("User not found");

        var resultList = new List<PostWatchHistory>();

        foreach (var id in ids)
        {

            var response = await _repository.AddWatchHistory(id, currentUser.Id);
            resultList.Add(response.Result);
        }
        return resultList;

    }

    async Task<ResponseBase<Post>> IPostService.AddPost(PostInput input)
    {
        User currentUser = GetCurrentUser();
        if (!_repository.Ratelimiter(currentUser.Id, "POST"))
        {
            var post = input.Adapt<Post>();
           // await _redisCache.SetAsync(DateTime.Now.ToLongTimeString()  , input);
            var jobId = BackgroundJob.Enqueue(
            () =>  SavePostAsync(input, currentUser));
            return new ResponseBase<Post>(post);
        }
        else
        {
            return CustomResponseStatus.Postlimit;
        }   

    }

    public async Task SavePostAsync(PostInput input, User currentUser)
    {
        var config = TypeAdapterConfig<Post, PostInput>
                          .NewConfig()
                          .Map(dest => dest.PostItems, src => src.PostItems)
                      .Map(dest => dest.Tags, src => src.Tags);



        if (!string.IsNullOrEmpty(input.Location))
        {
            AddLocation(input.Location);
        }
        int tagCounts = 0;
        if (input.Tags != null && input.Tags.Any())
        {
            input.Tags = input.Tags.Distinct().ToList();

            var tags = _tagRepository.GetUniqueTagsForPost(input.Tags);
            var Tags = new List<Tag>();
            foreach (var item in tags)
            {
                Tags.Add(new Tag
                {
                    Text = item,
                });
            }
            _repository.AddRange(Tags);
            input.StringTags = string.Join(",", input.Tags.ToList());
            tagCounts = Tags.Count;


        }

        var Links = new List<Link>();
        if (input.LinkInputs != null && input.LinkInputs.Any())
        {
            input.LinkInputs = input.LinkInputs.Distinct().ToList();

            var links = _linkRepository.GetUniqueLinksForPost(input.LinkInputs);
            foreach (var item in links)
            {
                item.LinkType = LinkType.Post;
                item.ArticleId = null;
                Links.Add(item.Adapt<Link>());
            }
            _repository.AddRange(Links);
        }

        input.PostItems = input.PostItems.Distinct().ToList();
        var post = input.Adapt<Post>();
        post.PostItemsString = JsonConvert.SerializeObject(input.PostItems);
        post.Links = Links;
        post.LatestUpdateThisWeeks = DateTime.UtcNow;
        _repository.Add(post);
        UpdateSettings(tagCounts, true, true);

        string[] usernames = input.YourMind.GetUsernames().Distinct().ToArray();
        var users = _userRepository.GetUserIdsFromUserNames(currentUser.Id, usernames);
        foreach (var item in users)
        {
            try
            {
                await _publisher.Publish(new MentionedInPostEvent(post, currentUser.Id, item));
            }
            catch
            {
            }
        }
        var applogs = new ApplicationLogs();
        SetCountUsedTag(input.Tags);
        applogs.RequestParameters = JsonConvert.SerializeObject(post);
        applogs.RequestName = "CompressionApi Call";
        var convertApiResponse = await _compressionApi.ConvertAsync(post);
        if (convertApiResponse != null && convertApiResponse.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var jsonResponse = convertApiResponse.Content.ReadAsStringAsync().Result;

            JObject data = JObject.Parse(jsonResponse);
            applogs.ResponseParameters = data.ToString();
            var strTaskId = data["task"]?["id"]?.ToString();
            applogs.RequesetId = strTaskId.ToString();
            _logs.Add(applogs);

            if (!string.IsNullOrEmpty(strTaskId))
            {
                int TaskId = Convert.ToInt32(strTaskId);
                await _repository.UpdatePostsCompreessedTaskId(TaskId, post.Id);
            }
        }
        else
        {
            applogs.ResponseParameters = JsonConvert.SerializeObject(convertApiResponse);
            try
            {
                var res = await _logs.AddAsync(applogs);
            }
            catch
            { }
        }
    }
    ResponseBase<PostAdsDto> IPostService.AddPostAds(PostAdsInput input)
    {
        var currentUser = GetCurrentUser();

        if (input.IsWithOutPayment && (currentUser == null || currentUser.UserTypes == UserTypes.User))
        {
            return ResponseStatus.NotEnoghData;
        }
        if (currentUser == null && input.IsWithOutPayment)
            return ResponseStatus.NotEnoghData;

        if (currentUser == null)
            currentUser = new User()
            {
                Email = "Guest User",
            };

        Discount discount = null;

        if (!string.IsNullOrEmpty(input.DiscountCode))
        {
            if (_userDiscountRepository.CheckIsDiscountAlreadyAppliedForUser(currentUser.Id, input.DiscountCode))
                return CustomResponseStatus.DiscountAlreadyUsed;
            discount = _discountRepository.GetDiscountByCode(input.DiscountCode).FirstOrDefault();


            if (discount is null)
                return CustomResponseStatus.DiscountNotFound;
            if (discount.ExpireDate < DateTime.Now)
                return CustomResponseStatus.DiscountExpire;
        }
        if (input.ManualStatus == ManualStatus.Manual && (input.TargetGenders is null
                   || input.TargetStartAge is null
                   || input.TargetEndAge is null
                   || string.IsNullOrEmpty(input.TargetLocation)))
        {
            return ResponseStatus.NotEnoghData;
        }

        try
        {
            _repository.BeginTransaction();

            var hasActiveAds = _repository.CheckIsPostActiveAds(input.Id ?? 0);
            if (hasActiveAds)
                return CustomResponseStatus.ThisPostHasActiveAdsOrActivePromote;

            _ = TypeAdapterConfig<Post, PostAdsInput>
                     .NewConfig()
                     .Map(dest => dest.PostItems, src => src.PostItems)
                     .Map(dest => dest.Tags, src => src.Tags);

            AddLocation(input.Location);

            int tagCounts = 0;
            if (input.Tags != null && input.Tags.Any())
            {
                input.Tags = input.Tags.Distinct().ToList();
                //  
                var tags = _tagRepository.GetUniqueTagsForPost(input.Tags);
                var Tags = new List<Tag>();
                foreach (var item in tags)
                {
                    Tags.Add(new Tag
                    {
                        Text = item,
                    });
                }

                _repository.AddRange(Tags);
                input.StringTags = string.Join(",", input.Tags.ToList());
                tagCounts = Tags.Count;
            }

            var post = input.Adapt<Post>();
            post.PostItemsString = JsonConvert.SerializeObject(input.PostItems);
            var ads = input.Adapt<Ads>();
            if (input.IsWithOutPayment)
            {
                ads.IsActive = true;
                ads.IsCompletedPayment = true;
                ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);

            }
            else
            {
                ads.IsActive = false;
                ads.IsCompletedPayment = false;
            }

            ads.Type = AdsType.Ads;
            ads.UserId = currentUser?.Id;
            post.Adses = new() { ads };

            post.LatestUpdateThisWeeks = DateTime.UtcNow;
            _repository.Add(post);
            UpdateSettings(tagCounts, true, false);

            if (input.IsWithOutPayment)
            {
                var user = _userRepository.GetUser(input.PosterId ?? 0).FirstOrDefault();
                ads.RaiseEvent(ref _events, currentUser, CrudType.AdsWithOutPayment);
                _eventStoreRepository.SaveEvents(_events);
                _repository.CommitTransaction();
                return new PostAdsDto(post, "");
            }
            var clientSecretResult = AddAdsPayment(currentUser, ref ads, discount);
            if (clientSecretResult.Status != ResponseStatus.Success)
            {
                _repository.RollBackTransaction();
                return ResponseStatus.Failed;
            }
            var result = new PostAdsDto(post, clientSecretResult?.Result);

            if (discount is not null)
                _repository.Add<UserDiscount>(new UserDiscount { UserId = currentUser.Id, DiscountId = discount.Id });


            _repository.CommitTransaction();
            return result;
        }
        catch
        {
            _repository.RollBackTransaction();
            return ResponseStatus.Failed;
        }
    }

    private void AddLocation(string location)
    {
        var placeExist = _placeRepository.isPlaceExists(location);
        if (!placeExist)
        {
            var newLocation = new Place { Location = location };
            _repository.Add(newLocation);
        }
    }

    private void UpdateSettings(int tagCounts, bool isTags, bool isReqularPost)
    {
        var settings = _settingRepository.GetFirstSettings();
        if (settings != null)
        {
            if (isReqularPost)
                settings.TotalPostsCount++;
            if (isTags)
                settings.TotalTagsCount += tagCounts;

            _repository.Update(settings);
        }
    }

    ResponseBase<PostAdsDto> IPostService.PromotePost(PromotePostInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        if (!currentUser.ProfessionalAccount)
            return CustomResponseStatus.JustProfessionalAccountCandPromotThePosts;

        if (input.ManualStatus == ManualStatus.Manual && (input.TargetGenders is null
                   || input.TargetStartAge is null
                   || input.TargetEndAge is null
                   || string.IsNullOrEmpty(input.TargetLocation)))
        {
            return ResponseStatus.NotEnoghData;
        }

        Discount discount = null;
        if (!string.IsNullOrEmpty(input.DiscountCode))
        {
            if (_userDiscountRepository.CheckIsDiscountAlreadyAppliedForUser(currentUser.Id, input.DiscountCode))
                return CustomResponseStatus.DiscountAlreadyUsed;

            discount = _discountRepository.GetDiscountByCode(input.DiscountCode).FirstOrDefault();


            if (discount is null)
                return CustomResponseStatus.DiscountNotFound;
            if (discount.ExpireDate < DateTime.Now)
                return CustomResponseStatus.DiscountExpire;
        }
        try
        {
            _repository.BeginTransaction();

            var post = _repository.GetPostById(input.PostId).FirstOrDefault();
            if (post == null)
                return ResponseStatus.NotFound;
            if (post.IsPromote)
                return CustomResponseStatus.AlreadyPromoted;

            var hasActiveAds = _repository.CheckIsPostActiveAds(input.PostId);
            if (hasActiveAds)
                return CustomResponseStatus.ThisPostHasActiveAdsOrActivePromote;

            var ads = input.Adapt<Ads>();
            ads.IsActive = false;
            ads.IsCompletedPayment = false;
            ads.Type = AdsType.PromotePost;
            ads.UserId = currentUser.Id;
            if (input.IsWithOutPayment)
                ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
            _repository.Add(ads);

            if (input.IsWithOutPayment)
            {
                var user = _userRepository.GetUser(post.PosterId).FirstOrDefault();
                ads.Post.Poster = user;
                ads.RaiseEvent(ref _events, currentUser, CrudType.AdsWithOutPayment);
                _eventStoreRepository.SaveEvents(_events);
                _repository.CommitTransaction();
                return new PostAdsDto(post, "");
            }
            var clientSecretResult = AddAdsPayment(currentUser, ref ads, discount);
            if (clientSecretResult.Status != ResponseStatus.Success)
            {
                _repository.RollBackTransaction();
                return ResponseStatus.Failed;
            }


            if (discount is not null)
                _repository.Add<UserDiscount>(new UserDiscount { UserId = currentUser.Id, DiscountId = discount.Id });

            var result = new PostAdsDto(post, clientSecretResult?.Result);

            _repository.CommitTransaction();
            return result;
        }
        catch (Exception)
        {
            _repository.RollBackTransaction();
            return ResponseStatus.Failed;
        }
    }

    private ResponseBase<string> AddAdsPayment(User currentUser, ref Ads ads, Discount discount = null)
    {
        var initialPrice = Convert.ToDouble(_configuration.GetSection("InitialPrice").Value);
        var numberOfPeoplerPerUnit = Convert.ToDouble(_configuration.GetSection("NumberOfPeoplePerUnit").Value);
        var adsCost = initialPrice * ads.NumberOfPeopleCanSee / numberOfPeoplerPerUnit;

        if (adsCost < 1 || adsCost * 100 > 99999999900)
            return CustomResponseStatus.InValidAmountForStripePayment;

        var paymentStatus = ads.Post.IsPromote ? PaymentStatus.PayForPromotePost : PaymentStatus.PayForPostAds;

        double amountdiscount = 0;
        if (discount is not null)
        {
            amountdiscount = discount.Amount > 0 ? discount.Amount : (((double)discount.Percent / (double)100) * adsCost);
            adsCost = adsCost - amountdiscount;

            if (adsCost < 0)
                adsCost = 0;
        }

        var payment = new Payment()
        {
            Amount = adsCost,
            Discount = amountdiscount,
            AmountWithoutDiscount = adsCost + amountdiscount,
            PaymentStatus = paymentStatus,
            UserId = currentUser?.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            AdsId = ads.Id,
            PaymentConfirmationStatus = PaymentConfirmationStatus.PenddingConfirmation,
        };

        if (adsCost == 0)
        {
            payment.PaymentConfirmationStatus = PaymentConfirmationStatus.Successful;
        }

        _repository.Add(payment);

        var paymentIntentInput = new PaymentIntentInput(
            "usd",
            adsCost,
            currentUser.StripeCustomerId,
            currentUser.Email,
            new Dictionary<string, string>() {
                    { "userId", currentUser?.Id.ToString() },
                    { "postId", ads.PostId.ToString()  },
                    { "adsId", ads.Id.ToString()  },
                    { "paymentId", payment?.Id.ToString() },
                    { "paymentStatus", paymentStatus.ToString() },
            });

        try
        {

            if (adsCost == 0)
            {

                ads.IsActive = true;
                ads.IsCompletedPayment = true;
                ads.TicketNumber = Guid.NewGuid().ToString().Substring(0, 12);
                _repository.Update(ads);
                return "";

            }
            else
            {

                var paymentIntent = _paymentService.CreatePaymentIntent(paymentIntentInput);
                if (paymentIntent.Status != ResponseStatus.Success)
                    return paymentIntent.Status;

                payment.PaymentIntentId = paymentIntent.Result.Id;
                _repository.Update(payment);

                ads.LatestPaymentIntentId = paymentIntent.Result.Id;
                ads.LatestPaymentDateTime = DateTime.UtcNow;
                ads.TotlaAmount = adsCost;

                _repository.Update(ads);
                return paymentIntent?.Result?.ClientSecret;
            }
        }
        catch
        {
            _repository.Remove(payment);
            return ResponseStatus.PaymentFailed;
        }
    }

    public override ResponseStatus SoftDelete(int entityId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        Post post = _repository.GetPostWithPoster(entityId);
        if (post == null)
            return ResponseBase<Post>.Failure(ResponseStatus.NotFound);

        if (currentUser.UserTypes == UserTypes.User && post.PosterId != currentUser?.Id)
            return ResponseBase<Post>.Failure(ResponseStatus.AuthenticationFailed);
        if (post.DeletedBy != DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyRemoved;
        }

        post.IsDeleted = true;

        if (post.PosterId == currentUser.Id)
            post.DeletedBy = DeletedBy.User;
        else
            post.DeletedBy = currentUser.UserTypes == UserTypes.User ? DeletedBy.User : DeletedBy.Admin;

        _repository.Update(post);
        var adses = _adRepository.GetAdsByPostId(post.Id);
        _repository.RemoveRange(adses);

        if (currentUser.UserTypes != UserTypes.User && post.PosterId != currentUser?.Id)
        {
            post.RaiseEvent(ref _events, currentUser, CrudType.DeletePost);
            _eventStoreRepository.SaveEvents(_events);
        }

        return ResponseStatus.Success;
    }

    public override ResponseBase<Post> Update(PostInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        Post val = input.Adapt<Post>();
        Post postFromDb = _repository.GetPostById(val.Id).FirstOrDefault();
        if (postFromDb == null)
        {
            return ResponseStatus.NotFound;
        }

        if (postFromDb.PosterId != currentUser.Id && !input.IsByAdmin)
            return ResponseStatus.AuthenticationFailed;

        try
        {
            _repository.BeginTransaction();

            input.PostItems = input.PostItems.Distinct().ToList();

            TypeAdapterConfig<Post, Post>
                             .NewConfig()
                             .Ignore(dest => dest.PostItems)
                             .Ignore(dest => dest.PosterId);

            val.Adapt(postFromDb);
            var postItems = input.PostItems.Adapt<List<PostItem>>();
            postFromDb.PostItems = postItems;
            postFromDb.PostItemsString = JsonConvert.SerializeObject(postItems);

            if (postFromDb.YourMind != input.YourMind)
            {
                postFromDb.LastModifiedDate = DateTime.UtcNow;
            }

            var placeExist = _placeRepository.isPlaceExists(input.Location);
            if (!placeExist)
            {
                var location = new Place { Location = input.Location };
                _repository.Add(location);
            }
            int tagCounts = 0;
            if (input.Tags != null && input.Tags.Any())
            {
                input.Tags = input.Tags.Distinct().ToList();

                var tags = _tagRepository.GetUniqueTagsForPost(input.Tags);
                var Tags = new List<Tag>();
                foreach (var item in tags)
                {
                    Tags.Add(new Tag
                    {
                        Text = item,
                    });
                }
                _repository.AddRange(Tags);
                input.StringTags = string.Join(",", input.Tags.ToList());

                postFromDb.StringTags = input.StringTags;
                tagCounts = Tags.Count;
                SetCountUsedTag(input.Tags);

            }
            var Links = new List<Link>();
            if (input.LinkInputs != null && input.LinkInputs.Any())
            {
                input.LinkInputs = input.LinkInputs.Distinct().ToList();
                var links = _linkRepository.GetUniqueLinksForPost(input.LinkInputs);
                foreach (var item in links)
                {
                    item.LinkType = LinkType.Post;
                    item.ArticleId = null;
                    item.PostId = postFromDb.Id;
                    Links.Add(item.Adapt<Link>());
                }
                _repository.AddRange(Links);
            }
            postFromDb.IsEdited = true;
            _repository.Update(postFromDb);
            var settings = _settingRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalTagsCount += tagCounts;
                _repository.Update(settings);
            }
            _repository.CommitTransaction();
            return postFromDb;
        }
        catch (Exception)
        {
            _repository.RollBackTransaction();
            return ResponseStatus.Failed;
        }
    }

    ResponseBase<Post> IPostService.PinPost(int postId, bool pin)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var post = _repository.GetPostById(postId).FirstOrDefault();
        if (post == null)
            return ResponseStatus.NotFound;

        if (post.PosterId != currentUser.Id)
            return ResponseStatus.NotAllowd;

        if (pin && _repository.GetTotalUserPinPostCount(currentUser.Id) == 4)
            return CustomResponseStatus.ExceedMaximmPinCount;

        post.PinDate = DateTime.UtcNow;
        post.IsPin = pin;
        return _repository.Update(post);
    }

    async Task<ResponseBase<Comment>> IPostService.CreateComment(CommentInput commentInput)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        if (commentInput.UserId is null || commentInput.UserId <= 0 || commentInput.PostId <= 0)
        {
            return ResponseBase<Comment>.Failure(ResponseStatus.NotEnoghData);
        }

        if (commentInput.CommentType == CommentType.Text && string.IsNullOrEmpty(commentInput.Text))
            return CustomResponseStatus.TextIsRequired;

        if (commentInput.CommentType != CommentType.Text && string.IsNullOrEmpty(commentInput.ContentAddress))
            return CustomResponseStatus.ContentAddressIsRequired;

        var post = _repository.GetPostById(commentInput.PostId).FirstOrDefault();//_repository.Find<Post>(commentInput.PostId);
        if (post == null) return ResponseStatus.NotFound;

        var isBlocked = _commentRepository.isUserBlockForPostComment(commentInput.UserId ?? 0, post.PosterId);
        if (isBlocked) return CustomMessagingResponseStatus.CanNotCommentToBlocker;

        var comment = commentInput.Adapt<Comment>();
        var commentResult = _repository.Add(comment);
        post.CommentsCount = _commentRepository.GetPostTotalCommentsCount(commentInput.PostId);

        await _repository.UpdatePostsEngagement(post);
        var settings = _settingRepository.GetFirstSettings();
        if (settings != null)
        {
            settings.TotalPostCommentsCount = _commentRepository.GetTotalCommentsCount();
            _repository.Update(settings);
        }

        await _interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = post.PosterId, UserId = currentUser.Id, PostId = post.Id, InterestedUserType = InterestedUserType.Post });

        string[] usernames = commentInput.Text.GetUsernames().Distinct().ToArray();
        var users = _userRepository.GetUserIdsFromUserNames(currentUser.Id, usernames);

        foreach (var item in users)
        {
            try
            {
                await _publisher.Publish(new MentionedInCommentEvent(commentResult, currentUser.Id, item));
            }
            catch
            {
            }
        }

        try
        {
            if (currentUser.Id != post.PosterId)
            {
                var poster = _userRepository.GetUser(post.PosterId).FirstOrDefault(); //_repository.Where<User>(c => c.Id == post.PosterId).FirstOrDefault();
                if (poster == null)
                    return ResponseStatus.NotFound;

                if (commentInput.ParentId != null)
                {
                    var parentCommenter = _commentRepository.GetComment(commentInput.ParentId ?? 0);

                    if (parentCommenter != null && currentUser.Id != parentCommenter.UserId && parentCommenter.User.CommentNotification)
                    {
                        await _publisher.Publish(new AddReplyCommentEvent(comment.Id, currentUser.Id, parentCommenter.UserId));
                    }
                }

                if (poster.CommentNotification)
                    await _publisher.Publish(new AddCommentEvent(comment.Id, currentUser.Id, post.PosterId, post.Id));
            }
        }
        catch
        {
        }

        return ResponseBase<Comment>.Success(commentResult);
    }
    public async Task<ResponseBase<List<UserViewPost>>> AddViews(List<int> postIds)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseBase<List<UserViewPost>>.Failure(ResponseStatus.NotFound);

        var posts = await _repository.GetPostsForAddViews(postIds, currentUser.Id);

        if (!posts.Any())
            return ResponseBase<List<UserViewPost>>.Failure(ResponseStatus.NotFound);

        var transaction = await _repository.BeginTransactionAsync();

        try
        {
            var viewResults = new List<UserViewPost>();

            foreach (var post in posts)
            {
                if (post.UserViewPosts.Count >= 1)
                    continue;

                var userViewPost = new UserViewPost()
                {
                    PostId = post.Id,
                    UserId = currentUser.Id,
                };

                post.Hits++;
                var viewResult = _repository.Add(userViewPost);
                viewResults.Add(viewResult);
                await _repository.UpdatePostsEngagement(post);

                if (post.PostType != PostType.RegularPost)
                {
                    var activeAdsOrPromote = post.Adses.FirstOrDefault();
                    if (activeAdsOrPromote != null)
                    {
                        activeAdsOrPromote.TotalViewed++;
                        if (activeAdsOrPromote.NumberOfPeopleCanSee <= activeAdsOrPromote.TotalViewed)
                        {
                            activeAdsOrPromote.IsActive = false;
                            if (post.IsPromote)
                                post.IsPromote = false;
                        }
                        _repository.Update(activeAdsOrPromote);
                    }
                }
            }

            await transaction.CommitAsync();
            return ResponseBase<List<UserViewPost>>.Success(viewResults);
        }
        catch
        {
            await transaction.RollbackAsync();
            return ResponseStatus.Failed;
        }
    }
    async Task<ResponseBase<PostLikes>> IPostService.LikePost(int userId, int postId, bool isLiked)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;
        var post = _repository.GetPostWithPoster(postId);
        if (post == null)
        {
            return ResponseStatus.NotFound;
        }

        if (!await _userRepository.isUserAvailable(userId))
        {
            return ResponseStatus.UserNotFound;
        }

        var postLike = _postLikeRepository.GetSinglePostLikesByPost(postId, userId);
        if (postLike == null && isLiked)
        {
            PostLikes entity = new PostLikes
            {
                UserId = userId,
                PostId = postId,
                Liked = true
            };
            postLike = _repository.Add(entity);
            post.LikesCount = _repository.GetSinglePostLikesCount(postId);
            await _repository.UpdatePostsEngagement(post);

            var settings = _settingRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalPostLikesCount = _postLikeRepository.GetTotalPostLikesCount();
                _repository.Update(settings);
            }

            await _interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = post.PosterId, UserId = currentUser.Id, PostId = post.Id, InterestedUserType = InterestedUserType.Post });
        }
        else if (postLike != null)
        {
            postLike.Liked = isLiked;
            var res = Update(postLike);
            post.LikesCount = _repository.GetSinglePostLikesCount(postId);
            await _repository.UpdatePostsEngagement(post);

            var settings = _settingRepository.GetFirstSettings();
            if (settings != null)
            {
                settings.TotalPostLikesCount = _postLikeRepository.GetTotalPostLikesCount();
                _repository.Update(settings);
            }

            if (isLiked)
            {
                await _interestedUserService.AddInterestedUser(new InterestedUserInput { FollowersUserId = post.PosterId, UserId = currentUser.Id, PostId = post.Id, InterestedUserType = InterestedUserType.Post });
            }
            else
            {
                var interestedUser = _repository.GetPostInterestedUser(post.Id, userId, post.PosterId);
                if (interestedUser != null)
                {
                    await _interestedUserService.DeleteInterestedUser(interestedUser.Id);
                }
            }

            SetCountLikeTag(post.Tags);
        }

        try
        {
            if (currentUser.Id != post.PosterId && post.Poster.LikeNotification)
                await _publisher.Publish(new LikePostEvent(postLike.Id, currentUser.Id, post.PosterId, postId));
        }
        catch
        {
        }

        SetCountLikeTag(post.Tags);

        return postLike;
    }

    async Task<ResponseBase> IPostService.UnLikePost(int userId, int postId)
    {
        PostLikes val = _repository.GetUserPostLikeWithPostDetails(postId, userId).FirstOrDefault();

        if (val == null)
            return ResponseBase.Failure(ResponseStatus.NotFound);

        _repository.Remove(val);
        val.Post.LikesCount = _repository.GetSinglePostLikesCount(postId);
        _repository.Update(val.Post);
        await _repository.UpdatePostsEngagement(val.Post);
        var settings = _settingRepository.GetFirstSettings();
        if (settings != null)
        {
            settings.TotalPostLikesCount = _repository.GetPostLikesCount();
            _repository.Update(settings);
        }

        SetCountLikeTag(val.Post.Tags);

        return ResponseBase.Success();
    }
    async Task<ResponseBase> IPostService.SavePost(int userId, int postId, bool liked)
    {
        var post = _repository.Find<Post>(postId);
        if (post == null)
        {
            return ResponseBase.Failure(ResponseStatus.NotFound);
        }

        if (!_repository.Any<User>(a => a.Id == userId))
        {
            return ResponseBase.Failure(ResponseStatus.UserNotFound);
        }
        var savePost = _repository.GetSavePost(postId, userId).FirstOrDefault();
        if (savePost != null)
        {
            return CustomResponseStatus.AlreadySaved;
        }

        var newSaveShop = new SavePost { UserId = userId, PostId = postId };
        post.SavePostsCount++;
        _repository.Add(newSaveShop);
        _repository.Update(post);
        await _repository.UpdatePostsEngagement(post);

        return ResponseBase.Success();
    }

    async Task<ResponseBase> IPostService.UnSavePost(int userId, int postId)
    {
        var saveShop = _repository.GetSavePost(postId, userId).FirstOrDefault();
        if (saveShop == null)
            return ResponseStatus.NotFound;

        saveShop.Post.SavePostsCount--;
        _repository.Remove(saveShop);
        await _repository.UpdatePostsEngagement(saveShop.Post);

        return ResponseBase.Success();
    }

    private User GetCurrentUser(string token = null)
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;
        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

    async Task<ResponseStatus> IPostService.UndoDeletePost(int entityId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        Post post = _repository.GetPostWithPoster(entityId);
        if (post == null)
            return ResponseBase<Post>.Failure(ResponseStatus.NotFound);

        if (post.DeletedBy == DeletedBy.NotDeleted)
        {
            return CustomResponseStatus.AlreadyUndo;
        }

        post.DeletedBy = DeletedBy.NotDeleted;
        await _repository.UpdateAsync(post);
        var adses = _repository.GetPostAds(post.Id);
        adses.ForEach(d => d.IsDeleted = false);
        await _adRepository.UpdateRangeAsync(adses);
        return ResponseStatus.Success;
    }


    private void SetCountUsedTag(List<string> tags)
    {
        if (tags is null)
            return;

        var rTags = _tagRepository.Where(d => tags.Contains(d.Text)).ToList();

        if (rTags.Count > 0)
        {
            foreach (var item in rTags)
            {
                item.UsesCount = _repository.GetTagUsesCount(item.Text);
            };

            _tagRepository.UpdateRange(rTags);
        }
    }

    private void SetCountLikeTag(List<string> tags)
    {
        if (tags is null)
            return;
        var rTags = _tagRepository.Where(d => tags.Contains(d.Text)).ToList();

        if (rTags.Count > 0)
        {
            foreach (var item in rTags)
            {
                item.LikesCount = _repository.GetLikeCount(item.Text);
            };

            _tagRepository.UpdateRange(rTags);
        }
    }
    public async Task UpdatePostsCompressedResponse(TaskResquestModel resquestModel, int PostId)
    {
        if (PostId > 0)
        {
            var log = new ApplicationLogs();
            log.RequestName = "Task Call Back API ";
            log.RequesetId = PostId.ToString();
            log.RequestParameters = JsonConvert.SerializeObject(resquestModel) + PostId;
            List<PostItem> items = new List<PostItem>();
            int? TaskId = 1;
            TaskId = _repository.GetPostTaskId(PostId);
            if (TaskId > 0)
            {
                var response = await _compressionApi.FollowupTaskDetails((int)TaskId);
                var jsonResponse = response.Content.ReadAsStringAsync().Result;
                JObject data = JObject.Parse(jsonResponse);
                var results = data["results"] as JArray;
                log.ResponseParameters = jsonResponse;
                await _logs.AddAsync(log);
                foreach (var item in resquestModel.results)
                {
                    var postitem = new PostItem();
                    postitem.Content = item.results.video;
                    postitem.ThumNail = item.results.thumbnail[item.results.thumbnail.Length - 1].URL;
                    postitem.Width = item.results.thumbnail[item.results.thumbnail.Length - 1].metadata.width;
                    postitem.Height = item.results.thumbnail[item.results.thumbnail.Length - 1].metadata.height;
                    items.Add(postitem);
                }

                if (items.Count() > 0)
                {
                    string PostItemString = JsonConvert.SerializeObject(items);
                    if (PostItemString is not null)
                    {
                        await _repository.UpdatePostsCompreessedResponse(PostItemString, PostId);
                    }
                }
            }
            else
            {
                log.ResponseParameters = "No Task aagainst this Post id " + PostId;
                await _logs.AddAsync(log);
            }
        }
    }
    public async Task<bool> redisPost(int currentUser)
    {
        var follower = _userRepository.GetUserFollowers().Where(c => c.Follower.Blocks.All(x => x.BlockedId != currentUser) && c.FollowedId == currentUser && c.FolloweAcceptStatus == FolloweAcceptStatus.Accepted).ToList();
        var followersId = follower.Select(f => f.FollowerId).ToList();
        string myCacheKey = $"post_Key{currentUser}";
        string myAdvanceCacheKey = $"Advance_Post{currentUser}";
        string cacheKeyDiscussion = $"discussions{currentUser}";
        await _redisCache.PublishUpdateAsync("cache_invalidation", myCacheKey);
        await _redisCache.PublishUpdateAsync("cache_invalidation", myAdvanceCacheKey);
        await _redisCache.PublishUpdateAsync("cache_invalidation", cacheKeyDiscussion);
        if (followersId != null)
        {
            foreach (var varitem in followersId)
            {
                string cacheKey = $"followers_Posts{varitem}";
                string advanceCacheKey = $"Advance_Post{varitem}";
                await _redisCache.PublishUpdateAsync("cache_invalidation", cacheKey);
                await _redisCache.PublishUpdateAsync("cache_invalidation", advanceCacheKey);
            }
            return true;
        }
        return false;
    }
    private sealed class TaskDisposable : IDisposable
    {
        private readonly Task _task;
        public TaskDisposable(Task task) => _task = task;
        public void Dispose() => _task.Dispose();
    }
}
