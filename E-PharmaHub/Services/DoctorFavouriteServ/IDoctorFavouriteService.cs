using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.DoctorFavouriteServ
{
    public interface IDoctorFavouriteService
    {
        Task<bool> AddToFavoritesAsync(string userId, int doctorId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int doctorId);
        Task<IEnumerable<DoctorReadDto>> GetUserFavoritesAsync(string userId);
    }
}
