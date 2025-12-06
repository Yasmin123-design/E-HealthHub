using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.CartItemRepo
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly EHealthDbContext _context;

        public CartItemRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task AddCartItemAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
        }

        public async Task RemoveCartItemAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
        }
        public async Task<List<CartItem>> GetCartItemsWithDetailsByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Medication)
                .Where(ci => ci.CartId == cartId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }


        public async Task<CartItem?> GetCartItemWithCartAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public void Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }


        public async Task<bool> IsOwnerAsync(int cartItemId, string userId)
        {
            return await _context.CartItems
                .Include(ci => ci.Cart)
                .AnyAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);
        }

    }
}
