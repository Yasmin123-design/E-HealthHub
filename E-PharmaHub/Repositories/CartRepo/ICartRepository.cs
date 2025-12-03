using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.CartRepo
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartItemsWithDetailsAsync(int cartId);

        Task<Cart> GetUserCartAsync(string userId, bool asNoTracking = false);
        Task AddCartItemAsync(CartItem item);
        Task RemoveCartItemAsync(CartItem item);
        Task ClearCartAsync(Cart cart);
        Task ClearCartItemsByPharmacyAsync(int cartId, int pharmacyId);
        Task AddAsync(Cart cart);
    }
}
