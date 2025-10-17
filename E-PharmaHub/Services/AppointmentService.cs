using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Appointment> BookAppointmentAsync(AppointmentDto dto)
        {
            var appointment = new Appointment
            {
                UserId = dto.UserId,
                DoctorId = dto.DoctorId,
                ClinicId = dto.ClinicId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Status = AppointmentStatus.Pending
            };

            return await _unitOfWork.Appointments.BookAppointmentAsync(appointment);
        }

    }
}
