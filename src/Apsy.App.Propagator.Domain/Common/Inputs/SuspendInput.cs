using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class SuspendInput : BaseInputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "{0} is required")]
    public int DayCount { get; set; }

    [GraphQLIgnore]
    public SuspendType SuspendType { get; set; }

    //public DateTime SuspensionLiftingDate { get; set; }
}