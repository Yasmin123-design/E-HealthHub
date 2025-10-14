using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IPharmacyService
    {
        Task<IEnumerable<PharmacySimpleDto>> GetAllPharmaciesAsync();
        Task<PharmacySimpleDto> GetPharmacyByIdAsync(int id);
        Task AddPharmacyAsync(Pharmacy pharmacy , IFormFile imageFile);
        Task UpdatePharmacyAsync(int id , Pharmacy pharmacy , IFormFile imageFile);
        Task DeletePharmacyAsync(int id);
        Task<bool> UpdatePharmacyAsync(string userId, PharmacyUpdateDto dto, IFormFile? image);
    }
}
