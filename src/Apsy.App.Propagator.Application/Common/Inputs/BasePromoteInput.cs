using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

public class BasePromoteInput : InputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public VisitType VisitType { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public string TargetLocation { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? TargetStartAge { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int? TargetEndAge { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public TargetGender? TargetGenders { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public ManualStatus ManualStatus { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int NumberOfPeopleCanSee { get; set; }
}
