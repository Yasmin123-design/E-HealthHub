using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Repositories.OrderRepo
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<IEnumerable<OrderResponseDto>> GetAllAsync();
        Task<OrderResponseDto?> GetOrderResponseByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(string userId);
        Task<IEnumerable<OrderResponseDto>> GetByPharmacyIdAsync(int pharmacyId);
        Task UpdateStatusAsync(int orderId, OrderStatus newStatus);
        Task MarkAsPaid(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetByPaymentIdEntityAsync(int paymentId);
        Task<Order?> GetPendingOrderEntityByUserAsync(string userId, int pharmacyId);
        Task UpdateAsync(Order order);
        Task<OrderResponseDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId);
        Task<OrderResponseDto?> GetByPaymentIdAsync(int paymentId);


    }
}
