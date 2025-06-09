namespace Apsy.App.Propagator.Api.RequestInterception;

public class Authentication
{
    public Authentication(ClaimsPrincipal claims, User currentUser)
    {
        Claims = claims;
        CurrentUser = currentUser;
    }

    public ClaimsPrincipal Claims { get; }
    public User CurrentUser { get; }
    public bool IsAuthenticated => Claims?.Identity?.IsAuthenticated ?? false;
    public bool IsAuthorized => CurrentUser != null;

    public bool UserIsActive => CurrentUser?.IsActive ?? false;
    public bool IsDeletedAccount => CurrentUser?.IsDeletedAccount ?? false;

    public bool IsSuspended => CurrentUser?.IsSuspended ?? false;
    public DateTime? SuspensionLiftingDate => CurrentUser?.SuspensionLiftingDate;

    public ResponseStatus Status
    {
        get
        {
            if (!IsAuthenticated) return ResponseStatus.AuthenticationFailed;
            if (!IsAuthorized) return ResponseStatus.UserNotFound;
            if (!UserIsActive) return CustomResponseStatus.UserIsNotActive;
            if (IsDeletedAccount) return CustomResponseStatus.UserAccountDeleted;
            if (IsSuspended && SuspensionLiftingDate > DateTime.UtcNow) return CustomResponseStatus.AccountAlreadySuspended;

            return ResponseStatus.Success;
        }
    }

    public void Deconstruct(out ClaimsPrincipal Claims, out User CurrentUser, out ResponseStatus Status)
    {
        Claims = this.Claims;
        CurrentUser = this.CurrentUser;
        Status = this.Status;
    }
}
