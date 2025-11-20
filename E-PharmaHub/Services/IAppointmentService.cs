using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IAppointmentService
    {
        Task<Appointment?> GetFullAppointmemtByIdAsync(int id);
        Task<AppointmentResponseDto> BookAppointmentAsync(AppointmentDto dto);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(string doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserAsync(string userId);
        Task<AppointmentResponseDto?> GetByIdAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
        Task<(bool success, string message)> ApproveAppointmentAsync(int appointmentId);
        Task<(bool success, string message)> RejectAppointmentAsync(int appointmentId);

    }
}
