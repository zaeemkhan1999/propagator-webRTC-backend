using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Apsy.App.Propagator.Domain.Enums;

public enum VerificationRequestAcceptStatus
{
    [Description("Pending")]
    [Display(Name = "Pending")]
    Pending,
    [Description("Accepted")]
    [Display(Name = "Accepted")]
    Accepted,
    [Description("Rejected")]
    [Display(Name = "Rejected")]
    Rejected,
}