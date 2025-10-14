using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IClinicService
    {
        Task<Clinic> CreateClinicAsync(Clinic clinic);
        Task<Clinic?> GetClinicByIdAsync(int id);
        Task<IEnumerable<Clinic>> GetAllClinicsAsync();
        Task<bool> UpdateClinicAsync(string userId, ClinicUpdateDto dto, IFormFile? image);
        Task<bool> DeleteClinicAsync(int id);
    }
}
