

namespace Propagator.Common.Services.Contracts
{
    public interface IJwtManagerService
    {
        Tokens GenerateTokenWithRefresh(AppUser user, List<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
