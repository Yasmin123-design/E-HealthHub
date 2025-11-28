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
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic.Name,
                ClinicImage = a.Clinic.ImagePath,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.UserName,
                DoctorImage = a.Doctor.ProfileImage,
                DoctorSpeciality = a.Doctor.DoctorProfile.Specialty,
                UserId = a.UserId,
                UserName = a.User.UserName,
                UserImage = a.User.ProfileImage,
                EndAt = a.EndAt,
                StartAt = a.StartAt,
                Status = a.Status              
            };
        }
    }

}
