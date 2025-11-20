using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPharmacyRepository : IGenericRepository<Pharmacy>
    {
        Task<IEnumerable<PharmacySimpleDto>> GetAllBriefAsync();
        Task<PharmacySimpleDto> GetByIdBriefAsync(int id);
        Task<Pharmacy?> GetPharmacyByPharmacistUserIdAsync(string userId);
        Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync(int count);
        Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(
    string medicationName, double userLat, double userLng);


    }
}
