using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> GetByIdAsync(int id);
        Task<IEnumerable<Prescription>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId);
        Task AddAsync(Prescription prescription);
        Task DeleteAsync(Prescription prescription);
    }
}
