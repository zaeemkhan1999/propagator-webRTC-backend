using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Domain.Common.Dtos.Inputs;

public class ActivationNotificationInput : InputDef
{
    [Required(ErrorMessage = "{0} is required")]
    public bool? DirectNotification { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public bool? FolloweBacknotification { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public bool? LikeNotification { get; set; }
    [Required(ErrorMessage = "{0} is required")]
    public bool? CommentNotification { get; set; }
}