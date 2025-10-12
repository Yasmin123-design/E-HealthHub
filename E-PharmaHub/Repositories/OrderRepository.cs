using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EHealthDbContext _context;
        public OrderRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .ToListAsync();
        }
        public async Task<Order?> GetPendingOrderByUserAsync(string userId, int pharmacyId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }

        public async Task MarkAsPaid(string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (order == null)
                throw new Exception("Doctor profile not found.");

            order.PaymentStatus = PaymentStatus.Paid;
        }
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(x => x.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
            }
        }
        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByPharmacyId(int pharmacyId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Medication)
                .Where(o => o.PharmacyId == pharmacyId)
                .ToListAsync();
        }
    }
}
