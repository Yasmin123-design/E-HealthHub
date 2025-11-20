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
        public async Task<AppointmentResponseDto> BookAppointmentAsync(AppointmentDto dto)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var clinic = await _unitOfWork.Clinics.GetByIdAsync(dto.ClinicId);
            if (clinic == null)
                throw new Exception("Clinic not found.");

            var appointment = new Appointment
            {
                UserId = dto.UserId,
                DoctorId = doctor.AppUserId,
                ClinicId = dto.ClinicId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Status = AppointmentStatus.Pending
            };

            var response = await _unitOfWork.Appointments
                .AddAppointmentAndReturnResponseAsync(appointment);

            return response;
        }


        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(string doctorId)
        {
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByDoctorIdAsync(doctorId);
            return appointments ?? Enumerable.Empty<AppointmentResponseDto>();
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserAsync(string userId)
        {
            var appointments = await _unitOfWork.Appointments.GetAppointmentsByUserIdAsync(userId);
            return appointments ?? Enumerable.Empty<AppointmentResponseDto>();
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Appointments.GetAppointmentResponseByIdAsync(id);
        }

        public async Task<Appointment?> GetFullAppointmemtByIdAsync(int id)
        {
            return await _unitOfWork.Appointments.GetByIdAsync(id);
        }

        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Completed;
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

            if (!appointment.PaymentId.HasValue)
                return (false, "No payment found for this appointment.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(appointment.PaymentId.Value);
            if (payment == null || string.IsNullOrEmpty(payment.PaymentIntentId))
                return (false, "Payment information missing or invalid.");

            var captured = await _stripePaymentService.CapturePaymentAsync(payment.PaymentIntentId);
            if (!captured)
                return (false, "Payment capture failed. Please verify payment status.");

            payment.Status = PaymentStatus.Paid;
            await _unitOfWork.CompleteAsync();

            appointment.Status = AppointmentStatus.Confirmed;
            appointment.IsPaid = true;
            await _unitOfWork.CompleteAsync();

            await _emailSender.SendEmailAsync(
                appointment.User.Email,
                "Appointment Approved",
                $"Hello {appointment.User.Email},<br/>Your appointment with Dr. {appointment.Doctor.UserName} has been approved successfully after confirming payment."
            );

            return (true, "Appointment approved successfully after confirming payment.");
        }

        public async Task<(bool success, string message)> RejectAppointmentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                return (false, "Appointment not found.");

            if (appointment.Status == AppointmentStatus.Cancelled)
                return (false, "Appointment already rejected/cancelled.");

            if (appointment.Status == AppointmentStatus.Confirmed)
                return (false, "Appointment already confirmed, cannot reject.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.IsPaid = false;
            await _unitOfWork.CompleteAsync();

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
                    else
                    {
                        return (false, "Payment refund failed. Please verify with Stripe.");
                    }
                }
            }

            await _emailSender.SendEmailAsync(
                appointment.User.Email,
                "Appointment Rejected",
                $"Hello {appointment.User.Email},<br/>Your appointment with Dr. {appointment.Doctor.UserName} has been rejected and payment refunded successfully."
            );

            return (true, "Appointment rejected successfully and payment refunded.");
        }

    }
}
