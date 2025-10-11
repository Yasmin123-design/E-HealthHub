using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task MarkAsPaid(string userId);
        Task<Order?> GetPendingOrderByUserAsync(string userId, int pharmacyId);

    }
}
