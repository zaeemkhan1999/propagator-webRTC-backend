namespace Apsy.App.Propagator.Domain.Enums;

public enum CrudType
{
    Add,
    Edit,
    Delete,

    RejectAds,
    UnRejectAds,
    SuspendAds,
    UnSuspendAds,

    RemoveComment,
    DeletePost,
    DeleteArticle,
    AccountBaned,
    DeleteAccount,
    AdsWithOutPayment,
    VerifyArticle,
    UsersSuspendedEvent,
    UsersUnSuspendedEvent,

    AddWarningAsBannerEvent,
    DeleteWarningAsBannerEvent
}
