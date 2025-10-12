using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IOrderService
    {
        Task<CartResult> CheckoutAsync(string userId, CheckoutDto dto);
        Task MarkAsPaid(string userId);
        Task<IEnumerable<BriefOrderDto>> GetAllOrdersAsync();
        Task<BriefOrderDto> GetOrderByIdAsync(int id);
        Task<bool> AcceptOrderAsync(int id);
        Task<bool> CancelOrderAsync(int id);
        Task<IEnumerable<BriefOrderDto>> GetOrdersByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<BriefOrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<BriefOrderDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId);
    }
}
