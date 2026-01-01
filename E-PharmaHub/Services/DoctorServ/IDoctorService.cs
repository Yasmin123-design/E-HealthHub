using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Services.DoctorServ
{
    public interface IDoctorService
    {
        Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile clinicImage, IFormFile doctorImage);
        Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorProfile?> GetDoctorDetailsByUserIdAsync(string userId);

        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);
        Task<DoctorProfile> GetDoctorByIdAsync(int id);
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync();
        Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdmin();
        Task<IEnumerable<DoctorSlotDto>> GetDoctorSlotsAsync(
     int doctorId,
     DateTime date);
        Task MarkAsPaid(string userId);
        Task<bool> UpdateDoctorProfileAsync(string userId, DoctorUpdateDto dto);
        Task DeleteDoctorAsync(int id);
        Task<IEnumerable<DoctorReadDto>> GetDoctorsAsync(Speciality? specialty,
    string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType);

        Task<(bool success, string message)> ApproveDoctorAsync(int doctorId);
        Task<(bool success, string message)> RejectDoctorAsync(int doctorId);
        Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync();
    }
}
