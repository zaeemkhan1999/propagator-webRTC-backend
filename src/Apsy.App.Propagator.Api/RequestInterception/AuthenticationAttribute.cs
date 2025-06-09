namespace Apsy.App.Propagator.Api.RequestInterception;

public class AuthenticationAttribute : GlobalStateAttribute
{
    public AuthenticationAttribute() : base("Authentication") { }
}