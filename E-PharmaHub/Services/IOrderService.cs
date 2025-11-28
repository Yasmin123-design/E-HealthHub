using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IOrderService
    {
        Task<CartResult> CheckoutAsync(string userId, CheckoutDto dto);
        Task MarkAsPaid(string userId);
        Task<(bool Success, string Message)> MarkAsDeliveredAsync(int orderId);

        Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<(bool Success, string Message)> AcceptOrderAsync(int id);
        Task<(bool Success, string Message)> CancelOrderAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId);
        Task<OrderResponseDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId);
    }
}
