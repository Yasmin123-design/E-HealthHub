using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IEmailSender _emailSender;

        public AppointmentService(IUnitOfWork unitOfWork , 
            IStripePaymentService stripePaymentService ,
            IEmailSender emailSender
            )
        {
            _unitOfWork = unitOfWork;
            _stripePaymentService = stripePaymentService;
            _emailSender = emailSender;
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

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(string doctorId)
        {
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByDoctorIdAsync(doctorId);

            if (!appointments.Any())
                return Enumerable.Empty<AppointmentResponseDto>();

            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorName = a.Doctor?.UserName ?? "N/A",
                UserName = a.User?.UserName ?? "N/A",
                ClinicName = a.Clinic?.Name ?? "N/A",
                StartAt = a.StartAt,
                EndAt = a.EndAt,
                Status = a.Status
            });
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserAsync(string userId)
        {
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByUserIdAsync(userId);

            if (!appointments.Any())
                return Enumerable.Empty<AppointmentResponseDto>();

            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorName = a.Doctor?.UserName ?? "N/A",
                UserName = a.User?.UserName ?? "N/A",
                ClinicName = a.Clinic?.Name ?? "N/A",
                StartAt = a.StartAt,
                EndAt = a.EndAt,
                Status = a.Status
            });
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return null;

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                DoctorName = appointment.Doctor?.UserName ?? "N/A",
                UserName = appointment.User?.UserName ?? "N/A",
                ClinicName = appointment.Clinic?.Name ?? "N/A",
                StartAt = appointment.StartAt,
                EndAt = appointment.EndAt,
                Status = appointment.Status
            };
        }

        public async Task<bool> UpdateStatusAsync(int id, AppointmentStatus status)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = status;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        public async Task<(bool success, string message)> ApproveAppointmentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                return (false, "Appointment not found.");

            if (appointment.Status == AppointmentStatus.Confirmed)
                return (false, "Appointment already approved.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return (false, "Appointment was cancelled before.");

            appointment.Status = AppointmentStatus.Confirmed;

            if (appointment.PaymentId.HasValue)
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(appointment.PaymentId.Value);
                if (payment != null && !string.IsNullOrEmpty(payment.PaymentIntentId))
                {
                    var captured = await _stripePaymentService.CapturePaymentAsync(payment.PaymentIntentId);
                    if (captured)
                    {
                        payment.Status = PaymentStatus.Paid;
                        appointment.IsPaid = true;
                        await _unitOfWork.CompleteAsync();
                    }
                    else
                    {
                        return (false, "Payment capture failed.");
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            await _emailSender.SendEmailAsync(
                appointment.User.Email,
                "Appointment Approved",
                $"Hello {appointment.User.Email},<br/>Your appointment with Dr. {appointment.Doctor.UserName} has been approved and payment captured successfully."
            );

            return (true, "Appointment approved and payment captured.");
        }

        public async Task<(bool success, string message)> RejectAppointmentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                return (false, "Appointment not found.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return (false, "Appointment already cancelled.");

            if (appointment.Status == AppointmentStatus.Confirmed)
                return (false, "Appointment already confirmed, cannot reject.");

            appointment.Status = AppointmentStatus.Cancelled;

            if (appointment.PaymentId.HasValue)
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(appointment.PaymentId.Value);
                if (payment != null && !string.IsNullOrEmpty(payment.PaymentIntentId))
                {
                    var canceled = await _stripePaymentService.CancelPaymentAsync(payment.PaymentIntentId);
                    if (canceled)
                    {
                        payment.Status = PaymentStatus.Refunded;
                        await _unitOfWork.CompleteAsync();
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            await _emailSender.SendEmailAsync(
                appointment.User.Email,
                "Appointment Rejected",
                $"Hello {appointment.User.Email},<br/>Your appointment with Dr. {appointment.Doctor.UserName} has been rejected and payment was refunded."
            );

            return (true, "Appointment rejected and payment refunded.");
        }

    }
}
