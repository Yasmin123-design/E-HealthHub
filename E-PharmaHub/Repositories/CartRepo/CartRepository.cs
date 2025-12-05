using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories.CartRepo
{
    public class CartRepository : ICartRepository
    {
        private readonly EHealthDbContext _context;

        public CartRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetUserCartAsync(string userId, bool asNoTracking = false)
        {
            IQueryable<Cart> query = _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Medication)
                        .ThenInclude(m => m.Inventories)
                        .ThenInclude(p => p.Pharmacy)
                .Where(c => c.UserId == userId);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public async Task RemoveCartItemAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
        }
        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }
        public async Task ClearCartAsync(Cart cart)
        {
            _context.CartItems.RemoveRange(cart.Items);
        }
        public async Task ClearCartItemsByPharmacyAsync(int cartId, int pharmacyId)
        {
            var items = await _context.CartItems
                .Include(i => i.Medication)
                    .ThenInclude(m => m.Inventories)
                .Where(i => i.CartId == cartId)
                .ToListAsync();

            var itemsToRemove = items
                .Where(i => i.Medication.Inventories
                    .Any(inv => inv.PharmacyId == pharmacyId && inv.Price == i.UnitPrice))
                .ToList();

            _context.CartItems.RemoveRange(itemsToRemove);
        }


        public async Task<List<CartItem>> GetCartItemsWithDetailsAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Medication)
                .Where(ci => ci.CartId == cartId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
