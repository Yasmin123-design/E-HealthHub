using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.PharmacyServ
{
    public interface IPharmacyService
    {
        Task<IEnumerable<PharmacySimpleDto>> GetAllPharmaciesAsync();
        Task<PharmacySimpleDto> GetPharmacyByIdAsync(int id);
        Task AddPharmacyAsync(Pharmacy pharmacy, IFormFile imageFile);
        Task DeletePharmacyAsync(int id);
        Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync();
        Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng);

        Task<(bool Success, string Message)> UpdatePharmacyAsync(int id, PharmacyUpdateDto dto, IFormFile? image);

    }
}
