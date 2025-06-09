using HotChocolate.AspNetCore;  
using HotChocolate.Execution;

namespace Apsy.App.Propagator.Api.RequestInterception;

public class CustomHttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
    {
        JwtUtilities jwtUtilities = context.RequestServices.GetService<JwtUtilities>()!;

        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var cp = jwtUtilities.GetClaimPrincipal(token);
        context.User = cp;

        if (context.User != null && context.User.Identity != null && context.User.Identity!.IsAuthenticated)
        {
            var appUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            using IServiceScope serviceScope = context.RequestServices.CreateScope();
            IDbContextFactory<DataContext> requiredService = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
            DataContext dataContext = requiredService.CreateDbContext();

            User currentUser = dataContext.Set<AppUser>().Where(c => c.Id == appUserId).Select(c => c.User)?.FirstOrDefault();
            var claims = new List<Claim>
            {
                new("user", JsonConvert.SerializeObject(currentUser))
            };

            var appIdentity = new ClaimsIdentity(claims);
            context.User.AddIdentity(appIdentity);
            requestBuilder.SetGlobalState("Authentication", new Authentication(context.User, currentUser));
        }
        else
        {
            requestBuilder.SetGlobalState("Authentication", new Authentication(context.User, null));
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}