using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IFavouriteDoctorRepository
    {
        Task<bool> AddToFavoritesAsync(string userId, int doctorId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int doctorId);
        Task<IEnumerable<DoctorReadDto>> GetUserFavoritesAsync(string userId);
        Task<int> CountByDoctorIdAsync(int doctorId);
    }
}
