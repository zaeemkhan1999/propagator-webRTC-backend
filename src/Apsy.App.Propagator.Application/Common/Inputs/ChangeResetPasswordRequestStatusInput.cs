using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class ChangeResetPasswordRequestStatusInput
{
    [Required(ErrorMessage = "{0} is  Required")]
    public int RequestId { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public ResetPasswordRequestStatus Status { get; set; }

    [Required(ErrorMessage = "{0} is  Required")]
    public string Password { get; set; }
}