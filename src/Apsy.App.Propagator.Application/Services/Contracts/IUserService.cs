namespace Apsy.App.Propagator.Application.Services.Contracts;

public interface IUserService : IUserServiceBase<User, UserInput>
{
    ResponseBase<User> SignUp(string externalId, string email, SignUpInput signUpInput);
    ResponseBase<User> ActivationNotification(ActivationNotificationInput input);
    ResponseBase<User> ActivationPrivateAccount(bool isActive);
    ResponseBase<User> ActivationTwoFactorAuthentication(bool isActive);
    Task<ResponseBase<bool>> VerificationTwoFactor(string username, string code);
    ResponseBase<Support> CreateSupport(SupportInput input);
    bool EmailExist(User currentUser, string email);
    ResponseBase<User> SwitchToProfessional();
    ResponseBase<ResponseStatus> SendEmail(EmailInput email);

    ValueTask<ResponseBase<User>> ActivationUser(bool isActive, int userId);
    ResponseBase<User> SetAsAdministrator(UserTypes userTypes, int userId);
    ValueTask<ResponseBase<bool>> AddPhoneNumber(string phoneNumber, string countryCode);
    ValueTask<ResponseBase<bool>> ConfirmPhoneNumber(string verificationCode);
    Task<ResponseBase<Tokens>> InsertUser(SignUpInput input);
    Task<ResponseBase<Tokens>> ConfirmEmail(string code, string email);
    Task<ResponseBase<bool>> ResendEmailConfirmation(ResendEmailConfirmationInput input);
    Task<ResponseBase<bool>> ChangePassword(ChangePasswordInput changePasswordInput, AppUser currentUser);
    Task<ResponseBase<bool>> ForgetPassword(ForgetPasswordInput forgetPassword);
    Task<ResponseBase<bool>> ResetPassword(ResetPasswordInput resetPasswordInput);
    Task<ResponseBase<Tokens>> Login(LoginInput input);
    Task<ResponseBase<Tokens>> LoginWithOTP(string code, string username);
    Task<ResponseBase<Tokens>> LoginWithSecurityAnswer(LoginWithSecurityAnswerInput input);
    Task<ResponseBase<Tokens>> RefreshToken(Tokens token);
    Task<ResponseBase<bool>> LogOut(string username);
    Task<ListResponseBase<UserClaimsViewModel>> UpdateUserPermission(PermissionInput input);
    Task<ResponseBase<User>> DeleteAccount(int userId);
    ResponseBase<User> SetAsAdmin(int userId, UserTypes userTypes);
    Task<ResponseBase<Tokens>> RegisterUserSecurityAnswer(List<RegisterWithSecurityAnswerInput> input, string username);
    Task<ResponseBase<bool>> ResetPasswordWithSecurityAnswer(ResetPasswordWithSecurityAnswerInput input);
    Task<ResponseBase> SetSubscriptionIdAsync(int userId, string subscriptionId);
    ResponseBase<User> SwitchToNormal();
    Task<ResponseBase<bool>> CreateResetPasswordRequest(ResetPasswordRequestInput input);
    Task<ResponseBase<bool>> ChangeResetPasswordRequestStatus(ChangeResetPasswordRequestStatusInput input);
    Task<ResponseBase> CancelSubscriptionAsync(int userId, int subscriptionPlanId);
    Task<ResponseBase> PauseSubscriptionAsync(int userId, int subscriptionPlanId);
    Task<ResponseBase> ResumeSubscriptionAsync(int userId, int subscriptionPlanId);
}