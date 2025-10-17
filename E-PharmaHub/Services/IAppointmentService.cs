using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> BookAppointmentAsync(AppointmentDto dto);
    }
}
