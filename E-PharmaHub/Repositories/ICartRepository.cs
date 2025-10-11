using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetUserCartAsync(string userId);
        Task AddCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(CartItem item);
        Task ClearCartAsync(Cart cart);
        Task AddAsync(Cart cart);
    }
}
