using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IDoctorRepository : IGenericRepository<DoctorProfile>
    {
        Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);

        Task<bool> ApproveDoctorAsync(int id);
        Task<bool> RejectDoctorAsync(int id);

    }
}
