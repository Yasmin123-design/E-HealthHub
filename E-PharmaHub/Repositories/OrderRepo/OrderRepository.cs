using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.OrderRepo
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

        public async Task UpdateStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _context.Orders.Update(order);
            }
        }

        public async Task MarkAsPaid(string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found.");

            order.PaymentStatus = PaymentStatus.Paid;
        }

        public async Task<Order?> GetByPaymentIdEntityAsync(int paymentId)
        {
            return await BaseOrderIncludes()
                .FirstOrDefaultAsync(o => o.PaymentId == paymentId);
        }

        private IQueryable<Order> BaseOrderIncludes()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Pharmacy)
                .Include(o => o.Payment)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .AsNoTracking();
        }

        private Expression<Func<Order, OrderResponseDto>> Selector =>
            OrderSelectors.GetOrderSelector();


        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            return await BaseOrderIncludes()
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<OrderResponseDto?> GetOrderResponseByIdAsync(int id)
        {
            return await BaseOrderIncludes()
                .Where(o => o.Id == id)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetByUserIdAsync(string userId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.UserId == userId)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetByPharmacyIdAsync(int pharmacyId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.PharmacyId == pharmacyId)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<OrderResponseDto?> GetByPaymentIdAsync(int paymentId)
        {
            return await BaseOrderIncludes()
                .Where(o => o.PaymentId == paymentId)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderResponseDto?> GetPendingOrderByUserAsync(string userId, int pharmacyId)
        {
            return await BaseOrderIncludes()
                .Where(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
        }
        public async Task<Order?> GetPendingOrderEntityByUserAsync(string userId, int pharmacyId)
        {
            return await BaseOrderIncludes()
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }
        public async Task<OrderResponseDto> AddOrderAndReturnResponseAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return await BaseOrderIncludes()
                .Where(o => o.Id == order.Id)
                .Select(Selector)
                .FirstAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                    .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.Pharmacy)
                .Include(o => o.Payment)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }


        public async Task<Order?> GetPendingOrderEntityByUserForUpdateAsync(string userId, int pharmacyId)
        {
            return await _context.Orders
                .Include(o => o.Items) 
                .FirstOrDefaultAsync(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);
        }

        public async Task<Order?> GetByIdForUpdateAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task UpdateWithItemsAsync(Order order)
        {
            _context.Orders.Update(order);

            foreach (var item in order.Items)
            {
                if (item.Id > 0)
                {
                    _context.Entry(item).State = EntityState.Modified;
                }
                else
                {
                    _context.OrderItems.Add(item);
                }
            }
        }

        public async Task<Order?> GetPendingOrderEntityByUserAsync(string userId, int pharmacyId, bool asNoTracking = false)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .Where(o =>
                    o.UserId == userId &&
                    o.PharmacyId == pharmacyId &&
                    o.Status == OrderStatus.Pending);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }
    }

}
