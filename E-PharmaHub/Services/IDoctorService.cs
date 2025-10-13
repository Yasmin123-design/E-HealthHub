using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IDoctorService
    {
        Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile image);
        Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty);
        Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId);
        Task<DoctorReadDto?> GetByIdDetailsAsync(int id);
        Task<DoctorProfile> GetDoctorByIdAsync(int id);
        Task MarkAsPaid(string userId);

        Task UpdateDoctorAsync(int id, DoctorProfile updatedDoctor, IFormFile? newImage);
        Task DeleteDoctorAsync(int id);

        Task<(bool success, string message)> ApproveDoctorAsync(int doctorId);
        Task<(bool success, string message)> RejectDoctorAsync(int doctorId);
    }
}
