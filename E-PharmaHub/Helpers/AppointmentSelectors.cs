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
                AppointmentAmount = a.Doctor.DoctorProfile.ConsultationPrice,
                ClinicImage = a.Clinic.ImagePath,
                DoctorId = a.Doctor.DoctorProfile.Id,
                DoctorName = a.Doctor.UserName,
                DoctorImage = a.Doctor.ProfileImage,
                DoctorSpeciality = a.Doctor.DoctorProfile.Specialty,
                UserId = a.UserId,
                UserNameLogged = a.User.UserName,
                UserImageLogged = a.User.ProfileImage,
                PatientAge = a.PatientAge,
                PatientPhone = a.PatientPhone,
                PatientName = a.PatientName,
                PatientGender = a.PatientGender,
                EndAt = a.EndAt,
                StartAt = a.StartAt,
                Status = a.Status              
            };
        }

        public static DoctorPatientDto ToDoctorPatientDto(Appointment appointment)
        {
            return new DoctorPatientDto
            {
                AppointmentId = appointment.Id,
                PatientName = appointment.PatientName,
                PatientPhone = appointment.PatientPhone,
                PatientAge = appointment.PatientAge,
                PatientGender = appointment.PatientGender,
                StartAt = appointment.StartAt,
                EndAt = appointment.EndAt,
                Status = appointment.Status,
                IsPaid = appointment.IsPaid,
                ClinicName = appointment.Clinic?.Name
            };
        }

    }

}
