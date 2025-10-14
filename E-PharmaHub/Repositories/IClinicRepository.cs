using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IClinicRepository : IGenericRepository<Clinic>
    {
        Task<Clinic> GetClinicByIdAsync(int? id);
        Task<Clinic?> GetClinicByDoctorUserIdAsync(string userId);

    }
}
