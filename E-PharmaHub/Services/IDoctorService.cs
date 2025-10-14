using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IDoctorService
    {
        Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile clinicImage, IFormFile doctorImage);
        Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorProfile?> GetDoctorDetailsByUserIdAsync(string userId);

        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);
        Task<DoctorProfile> GetDoctorByIdAsync(int id);
        Task MarkAsPaid(string userId);
        Task<bool> UpdateDoctorProfileAsync(string userId, DoctorUpdateDto dto, IFormFile? doctorImage);
        Task DeleteDoctorAsync(int id);

        Task<(bool success, string message)> ApproveDoctorAsync(int doctorId);
        Task<(bool success, string message)> RejectDoctorAsync(int doctorId);
    }
}
