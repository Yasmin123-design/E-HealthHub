using E_PharmaHub.Services.PaymentServ;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace E_PharmaHub.Controllers
{
    [Route("api/webhooks/stripe")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public StripeWebhookController(
            IPaymentService paymentService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _configuration["Stripe:WebhookSecret"],
                        throwOnApiVersionMismatch: false

                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook signature verification failed: {ex.Message}");
            }

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Session;
                    await _paymentService.HandleCheckoutSessionCompletedAsync(session);
                    break;

                case EventTypes.PaymentIntentCanceled:
                    var canceledIntent = stripeEvent.Data.Object as PaymentIntent;
                    await _paymentService.HandlePaymentCanceledAsync(canceledIntent);
                    break;
            }

            return Ok();
        }
    }
}
