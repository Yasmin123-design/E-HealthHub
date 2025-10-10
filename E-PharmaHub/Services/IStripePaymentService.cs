using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services
{
    public interface IStripePaymentService
    {
        Task<StripeSessionResponseDto> CreateCheckoutSessionAsync(PaymentRequestDto dto);
    }
}
