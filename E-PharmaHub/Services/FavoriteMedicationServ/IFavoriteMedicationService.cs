using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.FavoriteMedicationServ
{
    public interface IFavoriteMedicationService
    {
        Task<bool> AddToFavoritesAsync(string userId, int medicationId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int medicationId);
        Task<IEnumerable<MedicineDto>> GetUserFavoritesAsync(string userId);
    }
}
