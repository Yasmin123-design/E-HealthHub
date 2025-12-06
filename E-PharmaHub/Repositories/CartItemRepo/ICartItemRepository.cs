using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.CartItemRepo
{
    public interface ICartItemRepository
    {
        Task<List<CartItem>> GetCartItemsWithDetailsByCartIdAsync(int cartId);
        Task AddCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(CartItem item);

        Task<CartItem?> GetByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemWithCartAsync(int cartItemId);
        void Update(CartItem cartItem);
        Task<bool> IsOwnerAsync(int cartItemId, string userId);
    }
}
