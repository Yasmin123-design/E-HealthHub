using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Repositories.DoctorRepo
{
    public interface IDoctorRepository : IGenericRepository<DoctorProfile>
    {
        Task<IEnumerable<DoctorAvailability>> GetByDoctorAndDayAsync(
      int doctorId,
      DayOfWeek dayOfWeek);
        Task<DoctorProfile?> GetDoctorProfileByClinicIdAsync(int clinicId);
        Task<DoctorProfile?> GetDoctorProfileByIdAsync(int id);
        Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(Speciality specialty);
        Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorReadDto?> GetDoctorByUserIdReadDtoAsync(string userId);
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync();
        Task<IEnumerable<DoctorReadDto>> GetFilteredDoctorsAsync(Speciality? specialty,
              string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType);
        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);
        Task MarkAsPaid(string userId);
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdminAsync();
        Task<bool> ApproveDoctorAsync(int id);
        Task<bool> RejectDoctorAsync(int id);
        Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync(int count);
    }
}
