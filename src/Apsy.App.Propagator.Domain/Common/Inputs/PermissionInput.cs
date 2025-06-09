namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class PermissionInput : InputDef
{
    // public string RoleId { get; set; }
    public string Username { get; set; }
    public IList<UserClaimsViewModel> UserClaims { get; set; }
}

public class UserClaimsViewModel
{
    public string Type { get; set; }
    public string Value { get; set; }
    public bool Selected { get; set; }
}
