using E_PharmaHub.Dtos;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace E_PharmaHub.Services.CartServ
{
    public interface ICartService
    {
        Task<CartResult> AddToCartAsync(string userId, int pharmacyId, int medicationId, int quantity);
        Task<CartResult> RemoveFromCartAsync(string userId, int cartItemId);
        Task<CartResult> ClearCartAsync(string userId);
        Task<CartResponseDto> GetUserCartAsync(string userId);
    }
}
