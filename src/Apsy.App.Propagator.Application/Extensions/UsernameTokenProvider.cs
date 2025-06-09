using Microsoft.AspNetCore.Identity;

namespace Apsy.App.Propagator.Application.Extensions;

public class UsernameTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
{
    public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
        return Task.FromResult(true);
    }
}
