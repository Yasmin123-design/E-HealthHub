using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class AppointmentSelectors
    {
        public static Expression<Func<Appointment, AppointmentResponseDto>> GetAppointmentSelector()
        {
            return a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorName = a.Doctor.UserName,
                DoctorId = a.Doctor.Id,
                UserName = a.User.UserName,
                UserId = a.User.Id,
                ClinicName = a.Clinic.Name,
                ClinicId = a.Clinic.Id, 
                StartAt = a.StartAt,
                EndAt = a.EndAt,
                Status = a.Status
            };
        }
    }

}
