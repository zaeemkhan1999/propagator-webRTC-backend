using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Propagator.Common.Services.Contracts;

namespace Propagator.Common.Services
{
    public class JwtManagerService : IJwtManagerService
    {
        private readonly IConfiguration _configuration;

        public JwtManagerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public Tokens GenerateTokenWithRefresh(AppUser user, List<Claim> claims)
        {
            return GenerateJWTTokens(user, claims);
        }

        private Tokens GenerateJWTTokens(AppUser user, List<Claim> claims/*, List<Claim> authClaims*/)
        {

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
                var newClaims = new List<Claim>
                     {
                        new(ClaimTypes.NameIdentifier,user.Id),
                        new(ClaimTypes.Name, user.UserName),
                      };
                newClaims.AddRange(claims);


                var tokenExpireTime = Convert.ToInt32(_configuration["TokenExpireTime"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(newClaims),

                    Issuer = _configuration["JWT:Issuer"],
                    Audience = _configuration["JWT:Audience"],
                    //Claims = authClaims,

                    Expires = DateTime.UtcNow.AddMinutes(tokenExpireTime),
                    //Expires =  DateTime.Now.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                return new Tokens
                {
                    Access_Token = tokenHandler.WriteToken(token),
                    Refresh_Token = refreshToken,
                    ValidTo = token.ValidTo.ToUniversalTime()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
