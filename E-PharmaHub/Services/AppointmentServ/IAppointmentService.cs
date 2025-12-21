using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Services.AppointmentServ
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDto>> FilterByStatusAsync(
    AppointmentStatus status);
        Task<List<DoctorPatientDto>> GetDoctorPatientsAsync(string doctorId);
        Task<Appointment?> GetFullAppointmemtByIdAsync(int id);
        Task<AppointmentResponseDto> BookAppointmentAsync(AppointmentDto dto);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(string doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserAsync(string userId);
        Task<AppointmentResponseDto?> GetByIdAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
        Task<(bool success, string message)> ApproveAppointmentAsync(int appointmentId,string userId);
        Task<(bool success, string message)> RejectAppointmentAsync(int appointmentId);

    }
}
