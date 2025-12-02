using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.StripePaymentServ
{
    public interface IStripePaymentService
    {
        Task<StripeSessionResponseDto> CreateCheckoutSessionAsync(PaymentRequestDto dto);
        Task<bool> CapturePaymentAsync(string paymentIntentId);
        Task<bool> CancelPaymentAsync(string paymentIntentId);

    }
}
