using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services
{
    public interface IOrderService
    {
        Task<CartResult> CheckoutAsync(string userId, CheckoutDto dto);
        Task MarkAsPaid(string userId);
    }
}
