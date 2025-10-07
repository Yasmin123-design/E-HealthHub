using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IDoctorRepository : IGenericRepository<DoctorProfile>
    {
        Task<IEnumerable<DoctorProfile>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId);

    }
}
