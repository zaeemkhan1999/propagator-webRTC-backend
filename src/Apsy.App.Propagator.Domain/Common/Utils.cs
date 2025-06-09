namespace Apsy.App.Propagator.Domain.Common.Dtos;

public static class Utils
{

    private static IHttpContextAccessor httpContextAccessor;
    public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
    {
        httpContextAccessor = accessor;
    }


    public static User GetCurrentUser()
    {
        var User = httpContextAccessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }

    public static User GetCurrentUser(IHttpContextAccessor accessor)
    {
        var User = accessor.HttpContext.User;
        if (!User.Identity.IsAuthenticated)
            return null;

        var userString = User.Claims.FirstOrDefault(c => c.Type == "user")?.Value;
        var user = JsonConvert.DeserializeObject<User>(userString);
        return user;
    }


    public static string[] GetUsernames(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Array.Empty<string>();
        }

        string pattern = "@[a-zA-Z]+[a-zA-Z0-9._]*";
        Regex regex = new(pattern);

        return regex.Matches(input)
            .Select(x => x.Value[1..])
            .ToArray();
    }

    public static bool IsUserName(string input)
    {
        string pattern = "[a-zA-Z]+[a-zA-Z0-9._]*";
        Regex regex = new(pattern);
        Match match = regex.Match(input);
        return match.Success && match.Value == input;
    }

    public static int GetAge(this DateTime dateOfBirth)
    {
        var today = DateTime.Today;

        var a = (today.Year * 100 + today.Month) * 100 + today.Day;
        var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

        return (a - b) / 10000;
    }

}
