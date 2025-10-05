using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IClinicService
    {
        Task<Clinic> CreateClinicAsync(Clinic clinic);
        Task<Clinic?> GetClinicByIdAsync(int id);
        Task<IEnumerable<Clinic>> GetAllClinicsAsync();
        Task<bool> UpdateClinicAsync(Clinic clinic);
        Task<bool> DeleteClinicAsync(int id);
    }
}
