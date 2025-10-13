namespace E_PharmaHub.Services
{
    public interface IFavoriteMedicationService
    {
        Task<bool> AddToFavoritesAsync(string userId, int medicationId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int medicationId);
        Task<IEnumerable<object>> GetUserFavoritesAsync(string userId);
    }
}
