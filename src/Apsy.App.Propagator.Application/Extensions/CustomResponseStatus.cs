namespace Apsy.App.Propagator.Application.Extensions;

public record CustomResponseStatus : ResponseStatus
{
    public CustomResponseStatus(int code, string value) : base(code, value)
    {
    }

    public CustomResponseStatus(ResponseStatus original) : base(original)
    {
    }

    public static new ResponseStatus Failed = new CustomResponseStatus(0, "The operation was not successful.");
    public static new ResponseStatus Success = new CustomResponseStatus(1, "The operation was successful.");
    public static new ResponseStatus NotFound = new CustomResponseStatus(2, "The item you were looking for could not be found.");
    public static new ResponseStatus UnknownError = new CustomResponseStatus(3, "An unknown error occurred.");
    public static new ResponseStatus NotEnoghData = new CustomResponseStatus(4, "There is not enough data to complete the operation.");
    public static new ResponseStatus AuthenticationFailed = new CustomResponseStatus(5, "The authentication process failed.");
    public static new ResponseStatus UserNotFound = new CustomResponseStatus(6, "The user could not be found.");
    public static new ResponseStatus AlreadyExists = new CustomResponseStatus(7, "The item already exists.");
    public static new ResponseStatus AlreadyRemoved = new CustomResponseStatus(8, "The item has already been removed.");
    public static new ResponseStatus NotAllowd = new CustomResponseStatus(9, "You are not allowed to perform this operation.");
    public static new ResponseStatus TimeConflict = new CustomResponseStatus(11, "There is a conflict with the time you have chosen.");
    public static new ResponseStatus SessionNotFound = new CustomResponseStatus(12, "The session could not be found.");
    public static new ResponseStatus HostNotFound = new CustomResponseStatus(13, "The host could not be found.");
    public static new ResponseStatus StripeAccountNotExist = new CustomResponseStatus(14, "The Stripe account does not exist.");
    public static new ResponseStatus PaymentFailed = new CustomResponseStatus(15, "The payment was not successful.");
    public static new ResponseStatus FailedToWidthraw = new CustomResponseStatus(16, "The withdrawal failed.");
    public static new ResponseStatus SelfFollowingNotAllowed = new CustomResponseStatus(17, "You are not allowed to follow yourself.");
    public static new ResponseStatus AlreadyFollowed = new CustomResponseStatus(18, "You are already following this user.");
    public static new ResponseStatus InvalidTimeSyntax = new CustomResponseStatus(19, "The time syntax you have entered is invalid.");
    public static new ResponseStatus InvalidTimeRange = new CustomResponseStatus(20, "The time range you have entered is invalid.");
    public static new ResponseStatus DiffrenttIds = new CustomResponseStatus(21, "The IDs do not match.");
    public static new ResponseStatus AccountNeedsToHaveTransferEnabled = new CustomResponseStatus(22, "You need to enable transfers on your account to perform this operation.");
    public static new ResponseStatus RequiredDataNotFilled = new CustomResponseStatus(23, "You have not filled in all the required data.");

    public static ResponseStatus MessageUrlIsRequired = new CustomResponseStatus(101, "A link to the message is needed.");
    public static ResponseStatus UserIsNotActive = new CustomResponseStatus(102, "The user is not currently active.");
    public static ResponseStatus FailedToCreateCustomer = new CustomResponseStatus(103, "There was a problem creating a new customer.");
    public static ResponseStatus RequestToChatAlreadyExist = new CustomResponseStatus(104, "A request to start the chat already exists.");
    public static ResponseStatus AlreadySaved = new CustomResponseStatus(105, "This item has already been saved.");
    public static ResponseStatus InValidAmountForStripePayment = new CustomResponseStatus(106, "The payment amount entered for Stripe is not valid.");
    public static ResponseStatus PlatFormDontHaveEnoughBalanceInStripAccount = new CustomResponseStatus(107, "The platform’s Stripe account does not have enough funds.");
    public static ResponseStatus NotEnoughTokenExist = new CustomResponseStatus(108, "There are not enough tokens available.");
    public static ResponseStatus ProfileLocationIsRequired = new CustomResponseStatus(109, "A location for the profile is needed.");
    public static ResponseStatus LocationIsRequired = new CustomResponseStatus(110, "A location must be provided.");
    public static ResponseStatus LicenseIsRequired = new CustomResponseStatus(111, "A license is required.");
    public static ResponseStatus MedicalSystemNumberIsRequired = new CustomResponseStatus(112, "A medical system number must be provided. ");
    public static ResponseStatus InvalidDateRange = new CustomResponseStatus(113, "The date range entered is not valid.");
    public static ResponseStatus SelfBuyLiveNotAllowed = new CustomResponseStatus(114, "You cannot buy your own live content.");
    public static ResponseStatus WorkingHoursIsRequired = new CustomResponseStatus(115, "Working hours must be provided.");
    public static ResponseStatus TimeConfilict = new CustomResponseStatus(116, "There is a conflict with the time selected.");
    public static ResponseStatus SetForAtLeastTheNext2Weeks = new CustomResponseStatus(117, "This must be set for at least the next two weeks.");
    public static ResponseStatus InvalidStartDateOrEndDate = new CustomResponseStatus(118, "The start or end date entered is not valid.");
    public static ResponseStatus SelfBuySessionNotAllowed = new CustomResponseStatus(121, "You cannot buy your own session content.");
    public static ResponseStatus FailedToCreateConnectAccount = new CustomResponseStatus(122, "There was a problem creating a Connect account.");
    public static ResponseStatus TitleAlreadyExist = new CustomResponseStatus(123, "This title is already in use.");
    public static ResponseStatus UsernameAlreadyExist = new CustomResponseStatus(125, "This username is already taken.");
    public static ResponseStatus PostIsRequired = new CustomResponseStatus(126, "A post must be provided.");
    public static ResponseStatus ArticleIsRequired = new CustomResponseStatus(127, "An article must be provided.");
    public static ResponseStatus ContentAddressIsRequired = new CustomResponseStatus(128, "An address for the content is needed.");
    public static ResponseStatus TextIsRequired = new CustomResponseStatus(129, "Text must be entered.");
    public static ResponseStatus UserIsNotYourFollower = new CustomResponseStatus(130, "This user does not follow you.");
    public static ResponseStatus PhoneNumberAlreadyConfirmed = new CustomResponseStatus(131, "This phone number has already been verified.");
    public static ResponseStatus StripeAccountAlreadyExist = new CustomResponseStatus(132, "A Stripe account already exists for this user.");
    public static ResponseStatus FailedToOnboardingTheUser = new CustomResponseStatus(133, "There was a problem onboarding the user to the platform.");
    public static ResponseStatus ExceedMaximmPinCount = new CustomResponseStatus(134, "The maximum number of pins has been reached.");
    public static ResponseStatus StoryIdIsRequired = new CustomResponseStatus(135, "An ID for the story is needed.");
    public static ResponseStatus PostIdIsRequired = new CustomResponseStatus(136, "An ID for the post is needed.");
    public static ResponseStatus ArticleIdIsRequired = new CustomResponseStatus(137, "An ID for the article is needed.");
    public static ResponseStatus CanNotSendPrivateAccountContentToNonFollower = new CustomResponseStatus(138, "Private account content cannot be sent to non-followers.");
    public static ResponseStatus UserAlreadyBlockedByContentOwner = new CustomResponseStatus(139, "The user has been blocked by the owner of the content.");
    public static ResponseStatus EmailAlreadyExist = new CustomResponseStatus(140, "This email address is already in use.");
    public static ResponseStatus UsernameOrPasswordIsInvalid = new CustomResponseStatus(141, "The username or password entered is incorrect.");
    public static ResponseStatus InvalidCode = new CustomResponseStatus(142, "The code entered is not valid.");
    public static ResponseStatus EmailIsRequired = new CustomResponseStatus(143, "An email address must be provided.");
    public static ResponseStatus UserDontHavePassword = new CustomResponseStatus(144, "You do not have a password set up.");
    public static ResponseStatus IsLockedOut = new CustomResponseStatus(145, "Your account has been locked.");
    public static ResponseStatus RequiresTwoFactor = new CustomResponseStatus(146, "Two-factor authentication is required to access this account.");
    public static ResponseStatus ThisPostHasActiveAdsOrActivePromote = new CustomResponseStatus(147, "This post has active ads or is being actively promoted.");
    public static ResponseStatus ThisArticleHasActiveAdsOrActivePromote = new CustomResponseStatus(148, "This article has active ads or is being actively promoted.");
    public static ResponseStatus JustProfessionalAccountCandPromotThePosts = new CustomResponseStatus(149, "Only professional accounts are allowed to promote posts.");
    public static ResponseStatus AdsHasAlreadyComletedPayment = new CustomResponseStatus(150, "Payment for this ad has already been completed.");
    public static ResponseStatus AlreadyPromoted = new CustomResponseStatus(151, "This content has already been promoted.");
    public static ResponseStatus TheAdHasAlreadyBeenActivated = new CustomResponseStatus(152, "This ad has already been activated.");
    public static ResponseStatus UserAccountDeleted = new CustomResponseStatus(153, "The user’s account has been deleted.");
    public static ResponseStatus InvalidClaimType = new CustomResponseStatus(154, "The type of claim entered is not valid.");
    public static ResponseStatus InvalidClaimValue = new CustomResponseStatus(155, "The value entered for the claim is not valid.");
    public static ResponseStatus AnActiveWarningBannerAlreadyExist = new CustomResponseStatus(156, "An active warning banner already exists.");
    public static ResponseStatus LimitTheNumberOfStrike = new CustomResponseStatus(157, "Limit the number of strikes.");
    public static ResponseStatus LimitTheNumberOfBans = new CustomResponseStatus(158, "Limit the number of bans.");
    public static ResponseStatus LimitTheNumberOfSuspend = new CustomResponseStatus(159, "Limit the number of suspensions.");
    public static ResponseStatus ProfileIdIsRequired = new CustomResponseStatus(160, "A profile ID is required.");
    public static ResponseStatus AccountAlreadySuspended = new CustomResponseStatus(161, "The operation was not successful.");

    public static ResponseStatus InvalidPublicKey = new CustomResponseStatus(162, "public key is not valid.");
    public static ResponseStatus AnotherUserDontJoinedToSecretChat = new CustomResponseStatus(163, "another user dont joined To SecretChat .");
    public static ResponseStatus AlreadyUndo = new CustomResponseStatus(164, "This content has already been undo.");
    public static ResponseStatus DiscountAlreadyUsed = new CustomResponseStatus(165, "The discount code has already been used.");
    public static ResponseStatus DiscountExpire = new CustomResponseStatus(166, "The discount code has expired.");
    public static ResponseStatus DiscountNotFound = new CustomResponseStatus(167, "The discount code has notfound.");
    public static ResponseStatus AlreadyExistsSetAdmin = new CustomResponseStatus(168, "This user is already admin.");
    public static ResponseStatus AlreadyExistsReject = new CustomResponseStatus(169, "This item is already rejected.");
    public static ResponseStatus ErrorSendVerificationCode = new CustomResponseStatus(170, "There was an error sending the verification code, please check the phone number is correct and try again.");
    public static ResponseStatus InvalidAnswer = new CustomResponseStatus(171, "The answer entered is not valid.");
    public static ResponseStatus TheNotSetSecurityAnswer = new CustomResponseStatus(172, "The not set security answer.");

    public static ResponseStatus VideoDurationIsRequired = new CustomResponseStatus(173, "The video duration is required.");
    public static ResponseStatus Postlimit = new CustomResponseStatus(174, "Your daily limit for the Post has been exceed.");
    public static ResponseStatus ArticleLimit = new CustomResponseStatus(175, "Your daily limit for the Articles has been exceed");
    public static ResponseStatus AllowedUploadLimitExceed = new CustomResponseStatus(176, "File Size is More then allowed File Upload");
}