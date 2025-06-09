using Apsy.App.Propagator.Application.Authentication.Constants;

namespace Propagator.Api.Extensions
{
    public static class AuthorizationService
    {
        public static void AddMyAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.GiveStrikes, policy =>
                {
                    policy.RequireClaim(Permissions.GiveStrikes);
                });
                options.AddPolicy(Permissions.ViewPrivateGroups, policy =>
                {
                    policy.RequireClaim(Permissions.ViewPrivateGroups);
                });
                options.AddPolicy(Permissions.ViewPrivateAccounts, policy =>
                {
                    policy.RequireClaim(Permissions.ViewPrivateAccounts);
                });
                options.AddPolicy(Permissions.Demographics, policy =>
                {
                    policy.RequireClaim(Permissions.Demographics);
                });
                options.AddPolicy(Permissions.AdReports, policy =>
                {
                    policy.RequireClaim(Permissions.AdReports);
                });
                options.AddPolicy(Permissions.SuspendAds, policy =>
                {
                    policy.RequireClaim(Permissions.SuspendAds);
                });
                options.AddPolicy(Permissions.SetWarningAsInBanner, policy =>
                {
                    policy.RequireClaim(Permissions.SetWarningAsInBanner);
                });
                options.AddPolicy(Permissions.CreateAdsWithoutPayment, policy =>
                {
                    policy.RequireClaim(Permissions.CreateAdsWithoutPayment);
                });
                options.AddPolicy(Permissions.VerifyAccount, policy =>
                {
                    policy.RequireClaim(Permissions.VerifyAccount);
                });
                options.AddPolicy(Permissions.VerifyArticle, policy =>
                {
                    policy.RequireClaim(Permissions.VerifyArticle);
                });
                options.AddPolicy(Permissions.RejectAds, policy =>
                {
                    policy.RequireClaim(Permissions.RejectAds);
                });
                options.AddPolicy(Permissions.BanUsers, policy =>
                {
                    policy.RequireClaim(Permissions.BanUsers);
                });
                options.AddPolicy(Permissions.DeleteEntities, policy =>
                {
                    policy.RequireClaim(Permissions.DeleteEntities);
                });
            });
        }
    }
}
