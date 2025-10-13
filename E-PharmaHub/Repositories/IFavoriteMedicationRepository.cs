using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IFavoriteMedicationRepository
    {
        Task<bool> AddToFavoritesAsync(string userId, int medicationId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int medicationId);
        Task<IEnumerable<object>> GetUserFavoritesAsync(string userId);
    }
}
