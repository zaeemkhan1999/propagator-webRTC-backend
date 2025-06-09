using System.ComponentModel.DataAnnotations;

namespace Apsy.App.Propagator.Application.Common.Inputs;

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