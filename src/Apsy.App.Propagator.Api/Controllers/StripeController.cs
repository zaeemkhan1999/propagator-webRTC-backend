
using Apsy.App.Propagator.Application.Services.ReadContracts;
using Stripe;

namespace Apsy.App.Propagator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    private readonly IMessageService _messageService;
    private readonly IMessageReadService _messageReadService;
    private readonly IUsersSubscriptionService _usersSubscriptionService;
    private readonly IConfiguration _configuration;
    private readonly IPublisher _publisher;

    public StripeController(IPaymentService paymentService,
        IUserService userService,
        IMessageService messageService,
        IUsersSubscriptionService usersSubscriptionService,
        IMessageReadService messageReadService,
    IConfiguration configuration, IPublisher publisher)
    {
        _paymentService = paymentService;
        _userService = userService;
        _messageService = messageService;
        _messageReadService = messageReadService;
        _usersSubscriptionService = usersSubscriptionService;
        _configuration = configuration;
        _publisher = publisher;
    }

    [HttpPost("Webhook")]
    public async Task<IActionResult> Webhook()
    {
        try
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var secretKey = _configuration["Stripe:SigningSecret"];
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secretKey, tolerance: long.MaxValue, false);

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var metaData = paymentIntent!.Metadata;

                if (!metaData.Any())
                {
                    return Ok(false);
                }

                var paymentId = Convert.ToInt32(metaData["paymentId"]);

                if (metaData.ContainsKey("paymentStatus"))
                {
                    var paymentStatus = metaData["paymentStatus"].ToEnum<PaymentStatus>();
                    if (paymentStatus is PaymentStatus.PayForPromotePost or PaymentStatus.PayForPostAds)
                    {
                        var postId = Convert.ToInt32(metaData["postId"]);
                        var adsId = Convert.ToInt32(metaData["adsId"]);

                        await _paymentService.ConfirmPostPayment(postId, adsId, paymentIntent.Id, paymentId, PaymentConfirmationStatus.Successful, paymentStatus);
                    }

                    if (paymentStatus == PaymentStatus.PayForPromoteArticle)
                    {
                        var articleId = Convert.ToInt32(metaData["articleId"]);
                        var adsId = Convert.ToInt32(metaData["adsId"]);

                        await _paymentService.ConfirmArticlePayment(articleId, adsId, paymentIntent.Id, paymentId, PaymentConfirmationStatus.Successful, paymentStatus);
                    }
                }
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var metaData = paymentIntent!.Metadata;

                if (!metaData.Any())
                {
                    return Ok(false);
                }

                var paymentId = Convert.ToInt32(metaData["paymentId"]);

                if (metaData.ContainsKey("paymentStatus"))
                {
                    var paymentStatus = metaData["paymentStatus"].ToEnum<PaymentStatus>();
                    if (paymentStatus == PaymentStatus.PayForPromotePost || paymentStatus == PaymentStatus.PayForPostAds)
                    {
                        var postId = Convert.ToInt32(metaData["postId"]);
                        var adsId = Convert.ToInt32(metaData["adsId"]);

                        await _paymentService.FailPostPayment(postId, adsId, paymentIntent.Id, paymentId, PaymentConfirmationStatus.Failed, paymentStatus);
                    }

                    if (paymentStatus == PaymentStatus.PayForPromoteArticle)
                    {
                        var articleId = Convert.ToInt32(metaData["articleId"]);
                        var adsId = Convert.ToInt32(metaData["adsId"]);

                        await _paymentService.FailArticlePayment(articleId, adsId, paymentIntent.Id, paymentId, PaymentConfirmationStatus.Failed, paymentStatus);
                    }
                }
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var userId = int.Parse(subscription.Metadata["userId"]);
                var subscriptionPlanId = int.Parse(subscription.Metadata["subscriptionPlanId"]);


                //Environment is not set at the time of metadata, so it wont get here and creating issue so commented below code.
                //var envName = subscription!.Metadata["srvName"];
                //if (!envName.Equals(Environment.GetEnvironmentVariable("env")))
                //{
                //    return Ok(false);
                //}

                if (subscriptionPlanId > 0)
                {
                    int[] userIds = { userId };
                    var conversation = _messageReadService.GetConversationBySubscriptionPlanId(subscriptionPlanId);
                    if (conversation != null && conversation.Result != null)
                    {
                        int conversationId = conversation.Result.Id;
                        if (conversationId > 0 && conversation.Result.IsPrivate == true)
                        {
                            int adminId = conversation.Result.AdminId ?? 0;
                            //Add user to group who is subscribed. Here we passed userId=1 for superAdmin/groupOwner in AddUserToGroup method as per saksham instruction
                            await _messageService.AddUserToGroup(1, userIds, conversationId);
                            //for user
                            await _publisher.Publish(new AddUserToGroupEvent(conversationId, false, adminId, userId));
                            //for Admin
                            await _publisher.Publish(new AddUserToGroupEvent(conversationId, true, userId, adminId));
                        }
                    }
                }

                if (subscription.Status.Equals("active") ||
                    subscription.Status.Equals("incomplete"))
                {
                    await _userService.SetSubscriptionIdAsync(userId, subscription.Id);
                }
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var envName = subscription!.Metadata["env"];
                var userId = int.Parse(subscription.Metadata["userId"]);
                var subscriptionPlanId = int.Parse(subscription.Metadata["subscriptionPlanId"]);

                if (!envName.Equals(Environment.GetEnvironmentVariable("env")))
                {
                    return Ok(false);
                }

                await _userService.CancelSubscriptionAsync(userId, subscriptionPlanId);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionPaused)
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var envName = subscription!.Metadata["env"];
                var userId = int.Parse(subscription.Metadata["userId"]);
                var subscriptionPlanId = int.Parse(subscription.Metadata["subscriptionPlanId"]);

                if (!envName.Equals(Environment.GetEnvironmentVariable("env")))
                {
                    return Ok(false);
                }

                await _userService.PauseSubscriptionAsync(userId, subscriptionPlanId);
            }
            else if (stripeEvent.Type == Events.CustomerSubscriptionResumed)
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var envName = subscription!.Metadata["env"];
                var userId = int.Parse(subscription.Metadata["userId"]);
                var subscriptionPlanId = int.Parse(subscription.Metadata["subscriptionPlanId"]);

                if (!envName.Equals(Environment.GetEnvironmentVariable("env")))
                {
                    return Ok(false);
                }

                await _userService.ResumeSubscriptionAsync(userId, subscriptionPlanId);
            }
            else if (stripeEvent.Type == Events.InvoicePaid)
            {
                var invoice = (stripeEvent.Data.Object as Invoice)!;

                var envName = invoice.SubscriptionDetails.Metadata["env"];
                var userId = int.Parse(invoice.SubscriptionDetails.Metadata["userId"]);
                var subscriptionPlanId = int.Parse(invoice.SubscriptionDetails.Metadata["subscriptionPlanId"]);

                if (!envName.Equals(Environment.GetEnvironmentVariable("env")))
                {
                    return Ok(false);
                }

                await _usersSubscriptionService.ChargeUserSubscriptionPlanAsync(userId, subscriptionPlanId);
            }
            else
            {
                return BadRequest($"Unhandled event type: {stripeEvent.Type}");
            }

            return Ok(true);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
}
