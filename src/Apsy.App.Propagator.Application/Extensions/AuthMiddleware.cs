using RequestDelegate = Microsoft.AspNetCore.Http.RequestDelegate;

namespace Apsy.App.Propagator.Application.Extensions;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext httpContext, IUserRepository userRepo)
    {
        if (httpContext.User is not null && httpContext.User.Identity.IsAuthenticated)
        {
            var appUserId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = userRepo.GetUserByAppUserId(appUserId);

            var claims = new List<Claim>
                {
                    new Claim("user", JsonConvert.SerializeObject(user))
                };

            var appIdentity = new ClaimsIdentity(claims);
            httpContext.User.AddIdentity(appIdentity);
        }

        await _next(httpContext);
    }
}

