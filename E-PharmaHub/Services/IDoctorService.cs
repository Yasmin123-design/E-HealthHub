using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IDoctorService
    {
        Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId);
        Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile image);
        Task<DoctorProfile?> GetDoctorByIdAsync(int id);
        Task<IEnumerable<DoctorProfile>> GetDoctorsBySpecialtyAsync(string specialty);
        Task UpdateDoctorAsync(int id, DoctorProfile updatedDoctor, IFormFile? newImage);
        Task DeleteDoctorAsync(int id);

        //Task<bool> ApproveDoctorAsync(string doctorUserId);
    }
}
