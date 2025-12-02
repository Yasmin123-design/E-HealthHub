using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.PrescriptionServ
{
    public interface IPrescriptionService
    {
        Task<(bool success, string message, Prescription prescription)> CreatePrescriptionAsync(CreatePrescriptionDto dto);
        Task<IEnumerable<PrescriptionDetailsDto>> GetUserPrescriptionsAsync(string userId);
        Task<IEnumerable<PrescriptionDetailsDto>> GetDoctorPrescriptionsAsync(int doctorId);
        Task<PrescriptionDetailsDto?> GetByIdAsync(int id);
        Task<bool> DeletePrescriptionAsync(int id);
    }
}
