using System.IdentityModel.Tokens.Jwt;

namespace Apsy.App.Propagator.Api.RequestInterception;

public class JwtUtilities
{
    private readonly IConfiguration configuration;

    public JwtUtilities(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public ClaimsPrincipal GetClaimPrincipal(string jwtToken)
    {
        string secretKey = configuration["JWT:Key"];

        // Create token validation parameters
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,              // Validate the token issuer (optional)
            ValidateAudience = true,            // Validate the token audience (optional)
            ValidateLifetime = true,            // Validate the token expiration (optional)
            ValidateIssuerSigningKey = true,     // Validate the token signature (required)
            ValidIssuer = configuration["JWT:Issuer"],         // Specify the valid issuer (optional)
            ValidAudience = configuration["JWT:Audience"],     // Specify the valid audience (optional)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out _);
            return claimsPrincipal;
        }
        catch (Exception ex) when (ex is SecurityTokenValidationException or Exception)
        {
            ClaimsPrincipal claimsPrincipal = new();
            if (claimsPrincipal.Identity == null)
            {
                claimsPrincipal.AddIdentity(new ClaimsIdentity());
            }
            return claimsPrincipal;
        }
    }

    //public bool IsValidJwtToken(string jwtToken)
    //{
    //    ClaimsPrincipal claimsPrincipal = GetClaimPrincipal(jwtToken);

    //    return claimsPrincipal.Identity!.IsAuthenticated;
    //}
}
