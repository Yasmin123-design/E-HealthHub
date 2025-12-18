using E_PharmaHub.Models;
using Stripe;
using Stripe.Checkout;

namespace E_PharmaHub.Services.PaymentServ
{
    public interface IPaymentService
    {
        Task HandleCheckoutSessionCompletedAsync(Session session);
        Task HandlePaymentCanceledAsync(PaymentIntent paymentIntent);
        Task DeletePaymentAsync(Payment model);
        Task<object> VerifySessionAsync(string sessionId);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
    }
}
