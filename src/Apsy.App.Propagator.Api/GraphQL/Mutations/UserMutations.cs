using Apsy.App.Propagator.Application.Services.ReadContracts;

namespace Apsy.App.Propagator.Api.GraphQL.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    [GraphQLName("user_signUp")]
    public async Task<ResponseBase<Tokens>> SignUp(
                                [Service(ServiceKind.Default)] IUserService service,
                                [Service] INotificationService notificationService,
                                SignUpInput input)
    {
        //var usernameExist = service.UsernameExist(null, input.Username);
        //if (usernameExist)
        //    return CustomResponseStatus.UsernameAlreadyExist;

        //var emailExist = service.EmailExist(null, input.Email);
        //if (emailExist)
        //    return CustomResponseStatus.EmailAlreadyExist;
        //input.Username = "user_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        var signUpResponse = await service.InsertUser(input);
        // if (signUpResponse.Status == ResponseStatus.Success)
        // {
            
        //     var userId = signUpResponse.Result.UserId; 
            
        //     var welcomeNotification = new NotificationInput 
        //     {
               
        //         Text = "Welcome to Our Platform! Thank you for signing up. We're excited to have you onboard!",
                
        //     };
            
        //     await notificationService.SendNotificationToUser(userId, welcomeNotification);
        // }
        
        return signUpResponse;
    }

    [GraphQLName("user_confirmEmail")]
    public async Task<ResponseBase<Tokens>> ConfirmEmail(string code, string email, [Service] IUserService userService)
    {
        return await userService.ConfirmEmail(code, email);
    }

    [GraphQLName("user_resendEmailConfirmation")]
    public async Task<ResponseBase<bool>> ResendEmailConfirmation(ResendEmailConfirmationInput input, [Service] IUserService userService)
    {
        return await userService.ResendEmailConfirmation(input);
    }

    [GraphQLName("user_changePassword")]
    public async Task<ResponseBase<bool>> ChangePassword(
        ChangePasswordInput changePasswordInput,
        [Service] IUserReadService userReadService,
       [Service] IUserService userService,
        ClaimsPrincipal claims)
    {
        var currentUser = await userReadService.GetAppUser(claims);
        return await userService.ChangePassword(changePasswordInput, currentUser);
    }

    [GraphQLName("user_forgetPassword")]
    public async Task<ResponseBase<bool>> ForgetPassword(ForgetPasswordInput forgetPassword, [Service] IUserService userService)
    {
        return await userService.ForgetPassword(forgetPassword);
    }

    [GraphQLName("user_ResetPassword")]
    public async Task<ResponseBase<bool>> ResetPassword(ResetPasswordInput resetPasswordInput, [Service] IUserService userService)
    {
        return await userService.ResetPassword(resetPasswordInput);
    }

    [GraphQLName("user_resetPasswordWithSecurityAnswer")]
    public async Task<ResponseBase<bool>> ResetPassword(ResetPasswordWithSecurityAnswerInput input, [Service] IUserService userService)
    {
        return await userService.ResetPasswordWithSecurityAnswer(input);
    }

    [GraphQLName("user_createResetPasswordRequest")]
    public async Task<ResponseBase<bool>> CreateResetPasswordRequest(ResetPasswordRequestInput input,
        [Service] IUserService userService)
    {
        input.Status = ResetPasswordRequestStatus.NotChecked;
        return await userService.CreateResetPasswordRequest(input);
    }

    [GraphQLName("user_changeResetPasswordRequestStatus")]
    public async Task<ResponseBase<bool>> ChangeResetPasswordRequestStatus(ChangeResetPasswordRequestStatusInput input,
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IUserService userService)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        var currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await userService.ChangeResetPasswordRequestStatus(input);
    }

    [GraphQLName("user_registerUserSecurityAnswer")]
    public async Task<ResponseBase<Tokens>> RegisterUserSecurityAnswer(List<RegisterWithSecurityAnswerInput> input, string username, [Service] IUserService userService)
    {
        return await userService.RegisterUserSecurityAnswer(input, username);
    }

    [GraphQLName("user_login")]
    public async Task<ResponseBase<Tokens>> Login(LoginInput input,[Service] IUserService userService)
    {

        return await userService.Login(input);
    }

    [GraphQLName("user_loginWithOTP")]
    public async Task<ResponseBase<Tokens>> LoginWithOTP(LoginWithOTPInput input, [Service] IUserService userService)
    {
        return await userService.LoginWithOTP(input.Code, input.Username);
    }

    [GraphQLName("user_loginWithSecurityAnswer")]
    public async Task<ResponseBase<Tokens>> LoginWithSecurityAnswer(LoginWithSecurityAnswerInput input, [Service] IUserService userService)
    {
        return await userService.LoginWithSecurityAnswer(input);
    }

    [GraphQLName("user_refreshToken")]
    public async Task<ResponseBase<Tokens>> RefreshToken(Tokens token, [Service] IUserService userService)
    {
        return await userService.RefreshToken(token);
    }

    [GraphQLName("user_logOut")]
    public async Task<ResponseBase<bool>> LogOut(
                                [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                [Service] IUserService userService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        User currentUser = authentication.CurrentUser;
        return await userService.LogOut(currentUser.Username);
    }

    [GraphQLName("user_updateProfile")]
    public ResponseBase<User> UpdateProfile(
                              [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                              [Service] IUserService userService,
                              UserInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return userService.Update(input);
    }

    [GraphQLName("user_addPhoneNumber")]
    public async ValueTask<ResponseBase<bool>> AddPhoneNumber(
                        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                        [Service(ServiceKind.Default)] IUserService service,
                        string phoneNumber,
                        string countryCode)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.AddPhoneNumber(phoneNumber, countryCode);
    }

    [GraphQLName("user_confirmPhoneNumber")]
    public async ValueTask<ResponseBase<bool>> ConfirmPhoneNumber(
        string verificationCode,
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service(ServiceKind.Default)] IUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.ConfirmPhoneNumber(verificationCode);
    }

    [GraphQLName("user_activationNotifications")]
    public ResponseBase<User> ActivationNotification(
                                          [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service(ServiceKind.Default)] IUserService service,
                     ActivationNotificationInput input)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.ActivationNotification(input);
    }

    [GraphQLName("user_activationPrivateAccount")]
    public ResponseBase<User> ActivationPrivateAccount(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service(ServiceKind.Default)] IUserService service,
                     bool isActive)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.ActivationPrivateAccount(isActive);
    }

    [GraphQLName("user_activationTwoFactorAuthentication")]
    public ResponseBase<User> ActivationTwoFactorAuthentication(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service(ServiceKind.Default)] IUserService service,
                     bool isActive)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return service.ActivationTwoFactorAuthentication(isActive);
    }

    [GraphQLName("user_verificationTwoFactor")]
    public async Task<ResponseBase<bool>> VerificationTwoFactor(
                                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                                     [Service(ServiceKind.Default)] IUserService service,
                                     string username,
                                     string code)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }
        return await service.VerificationTwoFactor(username, code);
    }

    [GraphQLName("user_switchToProfessional")]
    public ResponseBase<User> SwitchToProfessional(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service(ServiceKind.Default)] IUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.SwitchToProfessional();
    }

    [GraphQLName("user_switchToNormal")]
    public ResponseBase<User> SwitchToNormal(
        [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
        [Service] IUserService service)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        return service.SwitchToNormal();
    }

    [GraphQLName("user_createSupport")]
    public ResponseBase<Support> CreateSupport(
                     [Service(ServiceKind.Default)] IUserService service,
                     SupportInput input)
                    => service.CreateSupport(input);

    [GraphQLName("user_setAsAdministrator")]
    public ResponseBase<User> SetAsAdministrator(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service] IUserService userService,
                     UserTypes userTypes,
                     int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.NotAllowd;

        if (userTypes == UserTypes.SuperAdmin)
            return ResponseStatus.NotAllowd;

        return userService.SetAsAdministrator(userTypes, userId);
    }

    [GraphQLName("user_banUser")]
    public async ValueTask<ResponseBase<User>> BanUser(
                     [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                     [Service] IUserService userService,
                     bool isActive,
                     int userId)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.SuperAdmin) return ResponseStatus.AuthenticationFailed;

        return await userService.ActivationUser(isActive, userId);
    }

    [GraphQLName("user_updateLastSeen")]
    public ResponseBase<User> UpdateLastSeen(
         [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
         [Service] IUserService userService)
    {
        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;

        currentUser.LastSeen = DateTime.UtcNow;
        return userService.Update(currentUser);
    }


    [GraphQLName("user_deleteAccount")]
    public async Task<ResponseBase<User>> DeleteAccount(
                 int? userId,
                 [Aps.CommonBack.Base.GraphQL.RequestInterception.Authentication] Authentication authentication,
                 [Service] IUserService service)
    {

        if (authentication.Status != ResponseStatus.Success)
        {
            return authentication.Status;
        }

        User currentUser = authentication.CurrentUser;
        if (currentUser.UserTypes != UserTypes.Admin && currentUser.UserTypes != UserTypes.User) return ResponseStatus.AuthenticationFailed;

        //int selectedUserId = currentUser.Id;
        //if ((currentUser.UserTypes == UserTypes.Admin || currentUser.UserTypes == UserTypes.User) && userId is not null)
        //{
        //    selectedUserId = (int)userId;
        //}

        return await service.DeleteAccount(currentUser.Id);
    }


}