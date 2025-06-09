
namespace Apsy.App.Propagator.Application.Authentication.Constants;

public static class Permissions
{
    public const string GiveStrikes = "Permissions.GiveStrikes";
    public const string ViewPrivateGroups = "Permissions.ViewPrivateGroups";
    public const string ViewPrivateAccounts = "Permissions.ViewPrivateAccounts";
    public const string Demographics = "Permissions.Demographics";
    public const string AdReports = "Permissions.AdReports";
    public const string SuspendAds = "Permissions.SuspendAds";
    public const string SetWarningAsInBanner = "Permissions.SetWarningAsInBanner";
    public const string CreateAdsWithoutPayment = "Permissions.CreateAdsWithoutPayment";
    public const string VerifyAccount = "Permissions.VerifyAccount";
    public const string VerifyArticle = "Permissions.VerifyArticle";
    public const string RejectAds = "Permissions.RejectAds";
    public const string BanUsers = "Permissions.BanUsers";
    public const string DeleteEntities = "Permissions.DeleteEntities";

    public static ResponseBase<bool> IsValidPermission(List<UserClaimsViewModel> input)
    {
        foreach (var item in input)
        {

            if (item.Value.ToLower() != "true" && item.Value.ToLower() != "false")
            {
                return CustomResponseStatus.InvalidClaimValue;
            }

            if (item.Type != GiveStrikes && item.Type != ViewPrivateGroups && item.Type != ViewPrivateAccounts
                && item.Type != Demographics && item.Type != AdReports && item.Type != SuspendAds
                && item.Type != SetWarningAsInBanner && item.Type != CreateAdsWithoutPayment && item.Type != VerifyAccount
                && item.Type != VerifyArticle && item.Type != BanUsers && item.Type != DeleteEntities)
            {
                return CustomResponseStatus.InvalidClaimType;
            }
        }
        return ResponseStatus.Success;
    }
}
