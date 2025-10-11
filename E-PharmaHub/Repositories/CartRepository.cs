using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly EHealthDbContext _context;

        public CartRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetUserCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(c => c.UserId == userId);
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
    }
}
