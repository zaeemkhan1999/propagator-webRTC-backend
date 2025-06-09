using Aps.CommonBack.Auth.Services;
using Apsy.App.Propagator.Domain.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Propagator.Common.Services.Contracts;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Text.Encodings.Web;
using Twilio.Rest.Verify.V2.Service;

namespace Apsy.App.Propagator.Application.Services;

public class UserService : UserServiceBase<User, UserInput>, IUserService
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserStore<AppUser> _userStore;
    private readonly IUserEmailStore<AppUser> _emailStore;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IUserRepository repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private List<BaseEvent> _events;
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IUserLoginRepository userLoginRepository;
    private readonly TwilioVerifySettingsVM _settings;
    private readonly IConfiguration _configuration;
    private readonly IJwtManagerService _jwtManagerService;
    //private readonly FirebaseAppCreator _firebaseApp;
    private readonly IWebHostEnvironment environment;
    private readonly string _confirmEmailUrl;
    private readonly string _resetpasswordUrl;
    private readonly IResetPasswordRequestRepository _resetPasswordRequestRepository;
    private readonly IUsersSubscriptionRepository _usersSubscriptionRepository;
    private readonly IPostRepository _postRepository;

    public UserService(
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore,
        SignInManager<AppUser> signInManager,
        //IEmailSender emailSender,
        RoleManager<IdentityRole> roleManager,

        IUserRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IEventStoreRepository eventStoreRepository,

        IOptions<TwilioVerifySettingsVM> settings,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        IJwtManagerService jwtManagerService,
        IUserLoginRepository userLoginRepository,
        IPostRepository postRepository,
        IResetPasswordRequestRepository resetPasswordRequestRepository,
        IUsersSubscriptionRepository usersSubscriptionRepository) : base(repository)
    //FirebaseAppCreator firebaseApp) : base(repository)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _roleManager = roleManager;

        this.repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _eventStoreRepository = eventStoreRepository;
        _events = new List<BaseEvent>();
        _postRepository = postRepository;
        _settings = settings?.Value;
        _configuration = configuration;
        this.environment = environment;
        _jwtManagerService = jwtManagerService;
        //_firebaseApp = firebaseApp;
        _confirmEmailUrl = _configuration["BaseUrl"] + _configuration["ConfirmEmailUrl"];
        _resetpasswordUrl = _configuration["BaseUrl"] + _configuration["ResetpasswordUrl"];
        this.userLoginRepository = userLoginRepository;
        _resetPasswordRequestRepository = resetPasswordRequestRepository;
        _usersSubscriptionRepository = usersSubscriptionRepository;
    }

    public async Task<ResponseBase> SetSubscriptionIdAsync(int userId, string subscriptionId)
    {
        var user = await repository
            .GetUser().Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        if (user == null)
        {
            return ResponseStatus.NotFound;
        }

        user.SubscriptionId = subscriptionId;

        try
        {
            await repository.UpdateAsync(user);

            return ResponseStatus.Success;
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }

    public async Task<ResponseBase> CancelSubscriptionAsync(int userId, int subscriptionPlanId)
    {
        var user = await repository
            .Where(x => x.Id == userId)
            .SingleOrDefaultAsync();

        if (user == null)
        {
            return ResponseStatus.NotFound;
        }

        user.SubscriptionId = null;

        var userSubscriptions = await repository.GetUsersSubscription()
            .Where(x => x.UserId == userId &&
                    x.SubscriptionPlanId == subscriptionPlanId &&
                    x.Status != UserSubscriptionStatuses.Canceled)
            .ToListAsync();

        foreach (var userSubscription in userSubscriptions)
        {
            userSubscription.Status = UserSubscriptionStatuses.Canceled;
        }

        var transaction = await repository.BeginTransactionAsync();

        try
        {
            await repository.UpdateAsync(user);
            await _usersSubscriptionRepository.UpdateRangeAsync(userSubscriptions);

            await transaction.CommitAsync();

            return ResponseStatus.Success;
        }
        catch
        {
            await transaction.RollbackAsync();
            return ResponseStatus.Failed;
        }
    }

    public async Task<ResponseBase> PauseSubscriptionAsync(int userId, int subscriptionPlanId)
    {
        var userSubscriptions = await repository.GetUsersSubscription()
            .Where(x => x.UserId == userId &&
                    x.SubscriptionPlanId == subscriptionPlanId &&
                    x.Status == UserSubscriptionStatuses.Active)
            .ToListAsync();

        foreach (var userSubscription in userSubscriptions)
        {
            userSubscription.Status = UserSubscriptionStatuses.Paused;
        }

        try
        {
            await _usersSubscriptionRepository.UpdateRangeAsync(userSubscriptions);
            return ResponseStatus.Success;
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }

    public async Task<ResponseBase> ResumeSubscriptionAsync(int userId, int subscriptionPlanId)
    {
        var userSubscriptions = await repository.GetUsersSubscription()
            .Where(x => x.UserId == userId &&
                    x.SubscriptionPlanId == subscriptionPlanId &&
                    x.Status != UserSubscriptionStatuses.Paused)
            .ToListAsync();

        foreach (var userSubscription in userSubscriptions)
        {
            userSubscription.Status = UserSubscriptionStatuses.Active;
        }

        try
        {
            await _usersSubscriptionRepository.UpdateRangeAsync(userSubscriptions);
            return ResponseStatus.Success;
        }
        catch
        {
            return ResponseStatus.Failed;
        }
    }

    public override ListResponseBase<User> Get(Expression<Func<User, bool>> predicate = null, bool checkDeleted = false)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var users = repository.GetUser();
        if (currentUser.UserTypes != UserTypes.Admin || currentUser.UserTypes != UserTypes.SuperAdmin)
            users = users.Where(c => !c.Blocks.Any(c => c.BlockedId == currentUser.Id)).AsQueryable();

        return new(users);
    }
    public async Task<ResponseBase<Tokens>> InsertUser(SignUpInput input)
    {
        if (!string.IsNullOrEmpty(input.Email))
        {
            var userExist = await _userManager.FindByEmailAsync(input.Email);
            if (userExist != null)
            {
                return CustomResponseStatus.EmailAlreadyExist;
            }
        }

        var usernameExist = await _userManager.FindByNameAsync(input.Username);
        if (usernameExist != null)
        {
            return CustomResponseStatus.UsernameAlreadyExist;
        }

        var user = CreateUser();
        user.UserName = input.Username;
        //user.TwoFactorEnabled = input.EnableTwoFactorAuthentication ?? false;
        user.SecurityStamp = Guid.NewGuid().ToString();
        var customUser = input.Adapt<User>();
        customUser.IsActive = true;
        customUser.IsDeletedAccount = false;
        user.User = customUser;
        user.User.NotInterestedPostIds = new List<int>();
        user.User.NotInterestedArticleIds = new List<int>();
        user.User.DirectNotification = true;
        user.User.CommentNotification = true;
        user.User.LikeNotification = true;
        user.User.FolloweBacknotification = true;
        user.User.UserTypes = UserTypes.User;
        //user.User.Ip = input.Ip;
        //add follow to owner
        var owner = repository.GetUser().Where(d => d.Email == _configuration["Owner:Email"]).FirstOrDefault();
        if (owner is not null)
        {
            user.User.Followees = new List<UserFollower>();
            user.User.Followees.Add(new UserFollower { FolloweAcceptStatus = FolloweAcceptStatus.Accepted, FollowedId = owner.Id, FollowedAt = DateTime.UtcNow });
        }

        await _userStore.SetUserNameAsync(user, input.Username, CancellationToken.None);

        if (!string.IsNullOrEmpty(user.Email))
        {
            await _emailStore.SetEmailAsync(user, input.Email, CancellationToken.None);
        }

        var result = await _userManager.CreateAsync(user, input.Password);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return new CustomResponseStatus(55555, error?.Description);
        }

        var tokenResult = await GenerateToken(user);
        return tokenResult;

    }

    public async Task<ResponseBase<bool>> ResendEmailConfirmation(ResendEmailConfirmationInput input)
    {
        if (string.IsNullOrEmpty(input.Email))
        {
            return CustomResponseStatus.EmailIsRequired;
        }

        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        await sendConfirmEmail(user);
        return ResponseBase<bool>.Success(true);
    }

    private async Task sendConfirmEmail(AppUser appUser)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var email = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(appUser.Email));
        var callbackUrl = $"{_confirmEmailUrl}?code={code}&Email={email}";

        var emailInput = new EmailInput
        {
            Subject = "Confirm your email",
            To = appUser.Email,
            HtmlContent = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body><div>Please confirm your account by <a clicktracking='off'  href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">clicking here</a></div></body></html>"
        };
        SendEmail(emailInput);
    }

    public async Task<ResponseBase<Tokens>> ConfirmEmail(string code, string email)
    {
        if (email == null || code == null)
        {
            return ResponseStatus.NotEnoghData;
        }

        var deEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email));

        var user = await _userManager.FindByEmailAsync(deEmail);
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return new CustomResponseStatus(777777, error?.Description);
        }

        var tokenResult = await GenerateToken(user);
        return tokenResult;
    }
    //set security answer next signup
    public async Task<ResponseBase<Tokens>> RegisterUserSecurityAnswer(List<RegisterWithSecurityAnswerInput> input, string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return ResponseStatus.UserNotFound;
        }

        if (input.Count < 3)
            return ResponseStatus.NotEnoghData;

        List<SecurityAnswer> lst = new();
        foreach (var item in input)
        {
            var securityAnswer = item.Adapt<SecurityAnswer>();

            securityAnswer.UserId = user.UserId;
            securityAnswer.SecurityQuestionId = item.QuestionId;
            lst.Add(securityAnswer);
        }

        await repository.AddRangeAsync(lst);

        var tokenResult = await GenerateToken(user);
        return tokenResult;
    }

    public async Task<ResponseBase<bool>> ChangePassword(ChangePasswordInput changePasswordInput, AppUser currentUser)
    {
        var User = _httpContextAccessor.HttpContext.User;
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return ResponseStatus.UserNotFound;

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
            return CustomResponseStatus.UserDontHavePassword;

        var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser, changePasswordInput.OldPassword, changePasswordInput.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            var error = changePasswordResult.Errors.FirstOrDefault();
            return new CustomResponseStatus(11111, error?.Description);
        }
        return ResponseBase<bool>.Success(true);
    }

    public async Task<ResponseBase<bool>> ForgetPassword(ForgetPasswordInput forgetPassword)
    {
        var user = await _userManager.FindByNameAsync(forgetPassword.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return ResponseBase<bool>.Success(false);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = $"{_resetpasswordUrl}?code={code}&Email={user.Email}";

        var emailInput = new EmailInput
        {
            Subject = "Reset Password",
            To = forgetPassword.Email,
            //HtmlContent = $"Please reset your password by <a clicktracking='off' href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">clicking here</a>.",
            HtmlContent = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body><div>Please reset your password by <a clicktracking='off' href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">clicking here</a></div></body></html>",
        };
        SendEmail(emailInput);

        return ResponseBase<bool>.Success(true);
    }

    public async Task<ResponseBase<bool>> ResetPasswordWithSecurityAnswer(ResetPasswordWithSecurityAnswerInput input)
    {
        var user = await _userManager.FindByNameAsync(input.Username);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return ResponseBase<bool>.Success(false);
        }

        var securityQuestionCount = await repository
            .GetSecurityAnswer().Where(d => (d.SecurityQuestionId == input.QuestionId1 && d.Answer == input.Answer1) ||
                                        (d.SecurityQuestionId == input.QuestionId2 && d.Answer == input.Answer2))
            .CountAsync();

        if (securityQuestionCount < 2)
            return CustomResponseStatus.InvalidAnswer;

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, code, input.Password);

        return ResponseBase<bool>.Success(result.Succeeded);
    }

    public async Task<ResponseBase<bool>> CreateResetPasswordRequest(ResetPasswordRequestInput input)
    {
        var user = await _userManager
            .Users
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.UserName == input.Username);

        if (user == null)
        {
            // Don't reveal that the user does not exist
            return ResponseBase<bool>.Failure(ResponseStatus.NotFound);
        }

        var request = input.Adapt<ResetPasswordRequest>();
        request.UserId = user.User.Id;

        try
        {
            await _resetPasswordRequestRepository.AddAsync(request);

            return ResponseBase<bool>.Success(true);
        }
        catch
        {
            return ResponseBase<bool>.Failure(ResponseStatus.UnknownError);
        }
    }

    public async Task<ResponseBase<bool>> ChangeResetPasswordRequestStatus(ChangeResetPasswordRequestStatusInput input)
    {
        var request = await _resetPasswordRequestRepository.GetByIdAsync(input.RequestId);
        if (request == null)
        {
            return ResponseBase<bool>.Failure(ResponseStatus.NotFound);
        }

        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return ResponseBase<bool>.Failure(ResponseStatus.NotFound);
        }

        var transaction = await _resetPasswordRequestRepository.BeginTransactionAsync();

        try
        {
            request.Status = input.Status;
            await _resetPasswordRequestRepository.UpdateAsync(request);

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, input.Password);

            await transaction.CommitAsync();
            return ResponseBase<bool>.Success(result.Succeeded);
        }
        catch
        {
            await transaction.RollbackAsync();
            return ResponseBase<bool>.Failure(ResponseStatus.UnknownError);
        }
    }
    public async Task<ResponseBase<bool>> ResetPassword(ResetPasswordInput resetPasswordInput)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordInput.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return ResponseBase<bool>.Success(true);
        }

        resetPasswordInput.Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordInput.Code));

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordInput.Code, resetPasswordInput.Password);
        if (result.Succeeded)
        {
            return ResponseBase<bool>.Success(true);
        }

        var error = result.Errors.FirstOrDefault();
        return new CustomResponseStatus(22222, error?.Description);
    }

    public async Task<ResponseBase<Tokens>> Login(LoginInput input)
    {
        var appUser = await _userManager
            .Users
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserName == input.Username);

        if (appUser == null)
            return ResponseStatus.UserNotFound;

        if (!appUser.User.IsActive)
        {
            return CustomResponseStatus.UserIsNotActive;
        }

        //if (!await repository.AnyAsync<SecurityAnswer>(d => d.UserId == user.UserId))
        //{
        //    return CustomResponseStatus.TheNotSetSecurityAnswer;
        //}

        await _signInManager.SignOutAsync();

        var signInResult = await _signInManager.PasswordSignInAsync(appUser, input.Password, false, true);

        if (appUser.TwoFactorEnabled && !signInResult.Succeeded && signInResult.RequiresTwoFactor)
        {
            //var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Username");
            //user.VerificationTwoFactorCode = token;
            //await _userManager.UpdateAsync(user);
            //string path = System.IO.Path.Combine(environment.WebRootPath, "EmailTemplates/2factor.txt");
            //string text = File.ReadAllText(path)
            //                .Replace("[Name]", user.Email)
            //                .Replace("[VerificationCode]", token);

            //string subject = "2 factor verification code!";
            //var emailInput = new EmailInput
            //{
            //    Subject = subject,
            //    To = input.Email,
            //    HtmlContent = text
            //    //HtmlContent = $"Please confirm your account by <a href='{{HtmlEncoder.Default.Encode(callbackUrl)}}'>clicking here</a>."
            //};
            //SendEmail(emailInput);
            return ResponseBase<Tokens>.Success(new Tokens { Access_Token = "CodeSent", Refresh_Token = "CodeSent" });
        }

        if (!signInResult.Succeeded)
        {
            if (signInResult.IsNotAllowed)
            {
                return ResponseStatus.NotAllowd;
            }
            if (signInResult.IsLockedOut)
            {
                return CustomResponseStatus.IsLockedOut;
            }
            //if (signInResult.RequiresTwoFactor)
            //{
            //    return CustomResponseStatus.RequiresTwoFactor;
            //}
            if (!await _userManager.IsLockedOutAsync(appUser))
            {
                await _userManager.AccessFailedAsync(appUser);
            }
        }

        var result = await _userManager.CheckPasswordAsync(appUser, input.Password);

        if (!result)
            return CustomResponseStatus.UsernameOrPasswordIsInvalid;

        var tokenResult = await GenerateToken(appUser);

        return tokenResult;
    }

    public async Task<ResponseBase<Tokens>> GenerateToken(AppUser user)
    {
        await _signInManager.RefreshSignInAsync(user);
        var calims = await _userManager.GetClaimsAsync(user);
        var tokenResult = _jwtManagerService.GenerateTokenWithRefresh(user, calims.ToList());
        if (tokenResult == null)
            return ResponseStatus.AuthenticationFailed;

        user.RefreshToken = tokenResult.Refresh_Token;

        await AddUserLogin(user);

        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.FirstOrDefault();
            return new CustomResponseStatus(999999, error?.Description);
        }

        return tokenResult;
    }

    public async Task<ResponseBase<Tokens>> LoginWithOTP(string code, string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        var signIn = await _signInManager.TwoFactorSignInAsync("Username", code, false, false);

        if (user == null)
            return ResponseStatus.UserNotFound;

        if (!signIn.Succeeded)
        {
            return CustomResponseStatus.InvalidCode;
        }

        var calims = await _userManager.GetClaimsAsync(user);

        var tokenResult = _jwtManagerService.GenerateTokenWithRefresh(user, calims.ToList());
        if (tokenResult == null)
            return ResponseStatus.AuthenticationFailed;

        user.RefreshToken = tokenResult.Refresh_Token;

        await AddUserLogin(user);
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.FirstOrDefault();
            return new CustomResponseStatus(999999, error?.Description);
        }

        return new(tokenResult);
    }

    public async Task<ResponseBase<Tokens>> RefreshToken(Tokens token)
    {
        var principal = _jwtManagerService.GetPrincipalFromExpiredToken(token.Access_Token);
        var username = principal.Identity?.Name;

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return ResponseStatus.UserNotFound;

        //if (user.RefreshToken != token.Refresh_Token)
        //    return ResponseStatus.AuthenticationFailed;

        var calims = await _userManager.GetClaimsAsync(user);

        var newJwtToken = _jwtManagerService.GenerateTokenWithRefresh(user, calims.ToList());
        if (newJwtToken == null)
            return ResponseStatus.AuthenticationFailed;

        user.RefreshToken = token.Refresh_Token;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault();
            return new CustomResponseStatus(55555, error?.Description);
        }

        return newJwtToken;
    }

    public async Task<ResponseBase<bool>> LogOut(string username)
    {
        await _signInManager.SignOutAsync();

        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser == null)
        {
            return ResponseStatus.NotFound;
        }

        var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        //var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

        var userLogins = await repository
                                  .GetUserLogin().Where(c => c.SigOutTime == null)
                                  .OrderByDescending(c => c.CreatedDate)
                                  .ToListAsync();

        foreach (var userLogin in userLogins)
        {
            userLogin.SigOutTime = DateTime.UtcNow;
            userLogin.SignOutIp = remoteIpAddress;
            repository.Update(userLogin);
        }

        return true;
    }

    public ResponseStatus IsAuthenticated(ResponseBase<AppUser> userAuth)
    {
        if (userAuth.Status != ResponseStatus.Success || userAuth.Result == null)
            return ResponseStatus.AuthenticationFailed;
        return ResponseStatus.Success;
    }

    private async Task<UserLogin> AddUserLogin(AppUser appUser)
    {
        var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

        var isSuspicious = await IsSuspicious(appUser, remoteIpAddress, userAgent);

        var userLogin = new UserLogin()
        {
            AppUserId = appUser.Id,
            SigInIp = remoteIpAddress,
            SigInTime = DateTime.UtcNow,
            UserAgent = userAgent,
            IsSuspicious = isSuspicious,
        };
        return await userLoginRepository.AddAsync(userLogin);
    }
    private  async Task<bool> IsSuspicious(AppUser appUser, string remoteIpAddress, string userAgent)
    {
        var exist = await userLoginRepository.AnyAsync(x => x.AppUserId == appUser.Id);
        if (!exist)
            return false;

        var newDevice = !(await userLoginRepository.AnyAsync(x => x.AppUserId == appUser.Id && x.UserAgent == userAgent));
        //var newIp = !repository.Any<UserLogin>(x => x.AppUserId == appUser.Id && (x.SigInIp == remoteIpAddress || x.SignOutIp == remoteIpAddress));

        return newDevice /*|| newIp*/;
    }

    //private JwtSecurityToken GetToken(List<Claim> authClaims)
    //{
    //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

    //    var token = new JwtSecurityToken(
    //        issuer: _configuration["JWT:Issuer"],
    //        audience: _configuration["JWT:Audience"],
    //        expires: DateTime.Now.AddDays(2),
    //        claims: authClaims,
    //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    //        );

    //    return token;
    //}

    private AppUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<AppUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<AppUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<AppUser>)_userStore;
    }

    public ResponseBase<User> SignUp(string externalId, string email, SignUpInput signUpInput)
    {
        ResponseBase<User> responseBase = RestoreUser(externalId);
        if ((ResponseStatus)responseBase == ResponseStatus.AlreadyExists || (ResponseStatus)responseBase == ResponseStatus.Success)
        {
            return responseBase;
        }

        User val = signUpInput.Adapt<User>();

        val.ExternalId = externalId;
        val.Email = email;
        val.IsActive = true;
        val.IsDeletedAccount = false;
        // val.Economy = new Economy();

        return repository.Add(val);
    }

    /// <summary>
    /// #change
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public new ResponseBase<User> Update(UserInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        if (input.Id != currentUser.Id)
            return ResponseStatus.AuthenticationFailed;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        if (UsernameExist(currentUser, input.Username))
            return CustomResponseStatus.UsernameAlreadyExist;
        input.Id = currentUser.Id;
        input.Adapt(userFromDb);

        return repository.Update(userFromDb);
    }
    public bool UsernameExist(User currentUser, string userName)
    {
        if (currentUser is null)
            return repository.GetUser().Any(c => !string.IsNullOrEmpty(c.Username) && c.Username == userName);
        return repository.GetUser().Any(c => c.Id != currentUser.Id
                        && !string.IsNullOrEmpty(c.Username) && c.Username == userName);
    }

    public async ValueTask<ResponseBase<User>> ActivationUser(bool isActive, int userId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var user = repository.GetById(userId, false);
        if (user == null)
            return ResponseBase<User>.Failure(ResponseStatus.NotFound);
        if (user.UserTypes == UserTypes.SuperAdmin)
            return ResponseBase<User>.Failure(ResponseStatus.NotAllowd);

        if (!isActive)
        {

            DateTime startDateTime = DateTime.Today; //Today at 00:00:00
            DateTime endDateTime = DateTime.Today.AddDays(1).AddTicks(-1); //Today at 23:59:59

            var adminTodayLimitation =
                    repository
                    .GetAdminTodayLimitation().Where(a => a.UserId == currentUser.Id && a.CreatedDate > startDateTime && a.CreatedDate < endDateTime)
                    .FirstOrDefault();

            if (currentUser.UserTypes == UserTypes.Admin && adminTodayLimitation != null && adminTodayLimitation.BansCount >= 20)
                return CustomResponseStatus.LimitTheNumberOfBans;

            if (adminTodayLimitation == null)
            {
                var newAdminTodayLimitation = new AdminTodayLimitation()
                {
                    UserId = currentUser.Id,
                    BansCount = 1
                };
                repository.Add(newAdminTodayLimitation);
            }

            if (adminTodayLimitation != null && adminTodayLimitation.BansCount < 20)
            {
                adminTodayLimitation.BansCount++;
                repository.Update(adminTodayLimitation);
            }
        }

        user.IsActive = isActive;
        var result = Update(user);

        user.RaiseEvent(ref _events, currentUser, isActive, CrudType.AccountBaned);
        await _eventStoreRepository.SaveEventsAsync(_events);

        await LogOut(user.Username);

        return result;
    }

    public ResponseBase<User> SetAsAdministrator(UserTypes userTypes, int userId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var user = repository.GetById(userId, false);
        if (user == null)
            return ResponseBase<User>.Failure(ResponseStatus.NotFound);
        if (user.UserTypes == UserTypes.SuperAdmin)
            return ResponseBase<User>.Failure(ResponseStatus.AuthenticationFailed);

        if (user.UserTypes == userTypes)
            return CustomResponseStatus.AlreadyExistsSetAdmin;

        user.UserTypes = userTypes;
        return repository.Update(user);
    }

    public async ValueTask<ResponseBase<bool>> AddPhoneNumber(string phoneNumber, string countryCode)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        if (repository.GetUser().Any(d => d.PhoneNumber == phoneNumber && d.PhoneNumberConfirmed == true))
            return ResponseStatus.AlreadyExists;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        if (userFromDb.PhoneNumberConfirmed)
            return CustomResponseStatus.PhoneNumberAlreadyConfirmed;

        userFromDb.PhoneNumber = phoneNumber;
        userFromDb.PhoneNumberConfirmed = false;
        userFromDb.CountryCode = countryCode;

        try
        {
            var verification = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: _settings.VerificationServiceSID
            );

            if (verification.Status == "pending")
            {
                repository.Update(userFromDb);
                return true;
            }

            return new CustomResponseStatus(100019, $"Wrong code! Please try again.");
        }
        catch
        {
            return CustomResponseStatus.ErrorSendVerificationCode;
        }
    }

    public async ValueTask<ResponseBase<bool>> ConfirmPhoneNumber(string verificationCode)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.NotFound;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        userFromDb.PhoneNumberConfirmed = true;
        try
        {
            var verification = await VerificationCheckResource.CreateAsync(
                to: userFromDb.PhoneNumber,
                code: verificationCode,
                pathServiceSid: _settings.VerificationServiceSID
            );
            if (verification.Status == "approved")
            {
                repository.Update(userFromDb);
                return true;
            }
            else
            {
                return new CustomResponseStatus(100020, $"Wrong code! Please try again.");
            }
        }
        catch
        {
            return new CustomResponseStatus(100020, $"There was an error confirming the code, please check the verification code is correct and try again");
        }
    }

    public bool EmailExist(User currentUser, string email)
    {
        if (currentUser is null)
            return repository.GetUser().Any(c => !string.IsNullOrEmpty(c.Username) && c.Email == email);
        return repository.GetUser().Any(c => c.Id != currentUser.Id
                        && !string.IsNullOrEmpty(c.Email) && c.Email == email);
    }

    public ResponseBase<User> ActivationNotification(ActivationNotificationInput input)
    {
        var currentUser = GetCurrentUser();
        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        userFromDb.DirectNotification = (bool)input.DirectNotification;
        userFromDb.FolloweBacknotification = (bool)input.FolloweBacknotification;
        userFromDb.LikeNotification = (bool)input.LikeNotification;
        userFromDb.CommentNotification = (bool)input.CommentNotification;
        return repository.Update(userFromDb);
    }

    public ResponseBase<User> ActivationPrivateAccount(bool isActive)
    {
        var currentUser = GetCurrentUser();
        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        userFromDb.PrivateAccount = isActive;
        return repository.Update(userFromDb);
    }

    public ResponseBase<User> ActivationTwoFactorAuthentication(bool isActive)
    {
        var currentUser = GetCurrentUser();
        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var userFromDb = repository
                            .GetUser().Where(c => c.Id == currentUser.Id)
                            .Include(x => x.AppUser)
                            .FirstOrDefault();

        if (userFromDb == null || userFromDb.AppUser == null)
            return ResponseStatus.UserNotFound;

        if (isActive)
        {
            Random _rdm = new Random();
            var code = _rdm.Next(10000, 99999);


            //var code = Guid.NewGuid().ToString();
            //var decodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = $"{TwoFactorUrl}?code={decodedCode}&Email={currentUser.Email}";

            //var emailInput = new EmailInput
            //{
            //    Subject = "Verification Two-factor authentication",
            //    To = currentUser.Email,
            //    HtmlContent = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body><div> confirm two-factor with this code : {code} </div></body></html>"
            //};
            //SendEmail(emailInput);
            userFromDb.AppUser.VerificationTwoFactorCode = code.ToString();
            userFromDb.AppUser.TwoFactorEnabled = true;
            userFromDb.AppUser.SecurityStamp = Guid.NewGuid().ToString();

        }
        else
        {
            userFromDb.AppUser.TwoFactorEnabled = isActive;
            userFromDb.AppUser.SecurityStamp = Guid.NewGuid().ToString();
        }

        repository.Update(userFromDb);
        return new(currentUser);
    }

    public async Task<ResponseBase<bool>> VerificationTwoFactor(string username, string code)
    {
        if (username == null || code == null)
        {
            return ResponseStatus.NotEnoghData;
        }

        var appUser = await _userManager.FindByNameAsync(username);
        if (appUser == null)
        {
            return ResponseStatus.UserNotFound;
        }

        if (appUser.VerificationTwoFactorCode != code)
        {
            return ResponseStatus.NotFound;
        }

        appUser.TwoFactorEnabled = true;
        appUser.SecurityStamp = Guid.NewGuid().ToString();

        await _userManager.UpdateAsync(appUser);

        return true;
    }

    public ResponseBase<User> SwitchToProfessional()
    {
        var currentUser = GetCurrentUser();
        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        userFromDb.ProfessionalAccount = true;
        return repository.Update(userFromDb);
    }

    public ResponseBase<User> SwitchToNormal()
    {
        var currentUser = GetCurrentUser();
        if (currentUser is null)
            return ResponseStatus.UserNotFound;

        var userFromDb = repository.GetUserByIdAsync(currentUser.Id);
        if (userFromDb == null)
            return ResponseStatus.NotFound;

        userFromDb.ProfessionalAccount = false;
        return repository.Update(userFromDb);
    }

    public ResponseBase<Support> CreateSupport(SupportInput input)
    {
        var user = repository.GetUser().Where(x => x.Email == input.Email).FirstOrDefault();
        if (user is not null)
            input.UserId = user.Id;

        var support = input.Adapt<Support>();
        return repository.Update(support);
    }

    private ResponseBase<User> RestoreUser(string externalId)
    {
        User byExternalIdWithDeleted = repository.GetByExternalIdWithDeleted(externalId);
        if (byExternalIdWithDeleted == null)
        {
            return ResponseBase<User>.Failure(ResponseStatus.NotFound);
        }

        if (byExternalIdWithDeleted.IsDeleted)
        {
            byExternalIdWithDeleted.IsDeleted = false;
            Update(byExternalIdWithDeleted);
            return ResponseBase<User>.Success(byExternalIdWithDeleted);
        }

        if (!byExternalIdWithDeleted.IsDeleted)
        {
            return ResponseBase<User>.Failure(ResponseStatus.AlreadyExists);
        }

        return ResponseBase<User>.Failure(ResponseStatus.UnknownError);
    }

    //public ResponseBase<Users> ConfirmationDoctor(int userId, DoctorAcceptStatus doctorAcceptStatus)
    // {
    //    var currentUser = GetCurrentUser();
    //    var userFromDb = repository.Find(userId);
    //    if (userFromDb == null)
    //    {
    //        return ResponseStatus.NotFound;
    //    }
    //    if (userFromDb.Id == currentUser.Id)
    //        return ResponseStatus.NotEnoghData;
    //    if (userFromDb.DoctorAcceptStatus != DoctorAcceptStatus.Pending)
    //        return ResponseStatus.NotEnoghData;
    //    userFromDb.DoctorAcceptStatus = doctorAcceptStatus;
    //    repository.Update(userFromDb);
    //    return userFromDb;
    //}

    //public ResponseBase<Users> UpdateDating(bool dating)
    //{
    //    var currentUser = GetCurrentUser();
    //    var userFromDb = repository.Find(currentUser.Id);
    //    if (userFromDb == null)
    //    {
    //        return ResponseStatus.NotFound;
    //    }
    //    userFromDb.Dating = dating;
    //    return repository.Update(userFromDb);
    //}
    public ResponseBase<ResponseStatus> SendEmail(EmailInput email)
    {
        try
        {
            var sendgridConfig = _configuration.GetSection("SendGridConfig").Get<SendGridConfig>();
            var client = new SendGridClient(sendgridConfig.ApiKey);
            var from = new EmailAddress(sendgridConfig.EmailFrom, "");
            var to = new EmailAddress(email.To, "");
            var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContent, email.HtmlContent);
            var response = client.SendEmailAsync(msg).ConfigureAwait(false).GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
                return ResponseBase<ResponseStatus>.Success(ResponseStatus.Success);
            else
                return ResponseBase<ResponseStatus>.Failure(ResponseStatus.Failed);
        }
        catch
        {
            return ResponseBase<ResponseStatus>.Failure(ResponseStatus.Failed);
        }
    }

    public async Task<ListResponseBase<UserClaimsViewModel>> UpdateUserPermission(PermissionInput input)
    {
        var isValidPermission = Permissions.IsValidPermission(input.UserClaims.ToList());
        if (isValidPermission.Status != ResponseStatus.Success)
            return isValidPermission.Status;

        var user = await _userManager.FindByNameAsync(input.Username);
        if (user == null)
            return ResponseStatus.NotFound;

        var claims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in claims)
        {
            await _userManager.RemoveClaimAsync(user, claim);
        }
        var selectedClaims = input.UserClaims.ToList();
        foreach (var claim in selectedClaims)
        {
            await _userManager.AddClaimAsync(user, new Claim(claim.Type, claim.Value));
        }
        return new(input.UserClaims.AsQueryable());
    }

    //public async Task<ListResponseBase<UserClaimsViewModel>> GeteUserPermission(string username)
    //{
    //    if (string.IsNullOrEmpty(username))
    //        return ResponseStatus.NotFound;

    //    var user = await _userManager.FindByNameAsync(username);
    //    if (user == null)
    //        return ResponseStatus.NotFound;

    //    var result = (await _userManager.GetClaimsAsync(user)).Select(g => new UserClaimsViewModel { Type = g.Type, Value = g.Value }).AsQueryable();

    //    return new(result);
    //}

    public async Task<ResponseBase<User>> DeleteAccount(int userId)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var user = repository.GetUserByIdAsync(userId);
        if (user == null)
            return ResponseStatus.UserNotFound;

        if (user.UserTypes == UserTypes.Admin || user.UserTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        if (!IsValidForDeleteAccount(user))
            return ResponseBase<User>.Failure(ResponseStatus.NotAllowd);

        //await _firebaseApp.DeleteUser(user.ExternalId);

        var result = await repository.DeleteUserAndDependencies(userId);

        if (currentUser.UserTypes != UserTypes.User && user.Id != currentUser?.Id)
        {
            user.RaiseEvent(ref _events, currentUser, false, CrudType.DeleteAccount);
            await _eventStoreRepository.SaveEventsAsync(_events);
        }

        return ResponseBase<User>.Success(result);
    }

    private bool IsValidForDeleteAccount(User user)
    {
        // var projectExist = repository.Any<Project>(c => c.UserId == user.Id && (c.ProjectStatus == ProjectStatus.InProgress));
        //var bidAsListerExist = repository.Any<Bid>(c => c.ListerId == user.Id && (c.BidStatus == BidStatus.InProgress || c.BidStatus == BidStatus.PenndingHuduWithdraw));
        //var bidAsHuduerExist = repository.Any<Bid>(c => c.HuduId == user.Id && c.BidStatus == BidStatus.InProgress);

        //var projectExist = true;
        //var bidAsListerExist = true;
        //var bidAsHuduerExist = true;

        //return !(projectExist || bidAsListerExist || bidAsHuduerExist);

        return true;
    }

    public ResponseBase<User> SetAsAdmin(int userId, UserTypes userTypes)
    {
        var currentUser = GetCurrentUser();
        if (currentUser == null)
            return ResponseStatus.AuthenticationFailed;

        var user = repository.GetUserByIdAsync(userId);
        if (user == null)
            return ResponseStatus.UserNotFound;
        if (user.UserTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;
        user.UserTypes = userTypes;
        repository.Update(user);
        return user;
    }

    private User GetCurrentUser()
    {
        var User = _httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

    public async Task<ResponseBase<Tokens>> LoginWithSecurityAnswer(LoginWithSecurityAnswerInput input)
    {
        var user = await _userManager
            .Users
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.UserName == input.Username);

        if (user == null)
            return ResponseStatus.UserNotFound;

        var securityQuestionCount = await repository
            .GetSecurityAnswer().Where(d => d.UserId == user.User.Id &&
            (
                (d.SecurityQuestionId == input.QuestionId1 && d.Answer == input.Answer1) ||
                (d.SecurityQuestionId == input.QuestionId2 && d.Answer == input.Answer2)
            ))
            .CountAsync();

        if (securityQuestionCount < 2)
            return CustomResponseStatus.InvalidAnswer;

        //var signIn = await _signInManager.TwoFactorSignInAsync("Username", user.VerificationTwoFactorCode, false, false);

        //if (!signIn.Succeeded)
        //{
        //    return CustomResponseStatus.InvalidCode;
        //}

        if (!await _signInManager.CanSignInAsync(user))
        {
            return ResponseStatus.AuthenticationFailed;
        }

        await _signInManager.SignInAsync(user, false);

        var calims = await _userManager.GetClaimsAsync(user);

        var tokenResult = _jwtManagerService.GenerateTokenWithRefresh(user, calims.ToList());
        if (tokenResult == null)
            return ResponseStatus.AuthenticationFailed;

        user.RefreshToken = tokenResult.Refresh_Token;

        await AddUserLogin(user);
        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            var error = identityResult.Errors.FirstOrDefault();
            return new CustomResponseStatus(999999, error?.Description);
        }

        return new(tokenResult);
    }

}
