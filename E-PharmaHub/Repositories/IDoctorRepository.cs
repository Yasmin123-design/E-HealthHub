using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IDoctorRepository : IGenericRepository<DoctorProfile>
    {
        Task<DoctorProfile?> GetDoctorProfileByIdAsync(int id);
        Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorReadDto?> GetDoctorByUserIdReadDtoAsync(string userId);
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync();
        Task<IEnumerable<DoctorReadDto>> GetFilteredDoctorsAsync(
              string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType);
        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);
        Task MarkAsPaid(string userId);
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdminAsync();
        Task<bool> ApproveDoctorAsync(int id);
        Task<bool> RejectDoctorAsync(int id);
    }
}
