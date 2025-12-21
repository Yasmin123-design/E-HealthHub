using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.EmailSenderServ;
using E_PharmaHub.Services.FileStorageServ;
using E_PharmaHub.Services.PaymentServ;
using E_PharmaHub.Services.StripePaymentServ;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services.DoctorServ
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileStorageService _fileStorage;
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IPaymentService _paymentService;
        private readonly IEmailSender _emailSender;

        public DoctorService(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IFileStorageService fileStorage,
            IStripePaymentService stripePaymentService,
            IPaymentService paymentService,
            IEmailSender emailSender)

        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileStorage = fileStorage;
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
            _emailSender = emailSender;
        }


        public async Task<IEnumerable<DoctorSlotDto>> GetDoctorSlotsAsync(
            int doctorId,
            DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;

            var availabilities =
                await _unitOfWork.Doctors
                    .GetByDoctorAndDayAsync(doctorId, dayOfWeek);

            if (!availabilities.Any())
                return Enumerable.Empty<DoctorSlotDto>();

            var appointments =
                await _unitOfWork.Appointments
                    .GetConfirmedByDoctorAndDateAsync(doctorId, date);

            var result = new List<DoctorSlotDto>();

            foreach (var avail in availabilities)
            {
                var start = date.Date + avail.StartTime;
                var end = date.Date + avail.EndTime;

                while (start.AddMinutes(avail.SlotDurationInMinutes) <= end)
                {
                    var slotEnd = start.AddMinutes(avail.SlotDurationInMinutes);

                    var isBooked = appointments.Any(a =>
                        a.StartAt == start &&
                        a.EndAt == slotEnd);

                    result.Add(new DoctorSlotDto
                    {
                        StartAt = start,
                        EndAt = slotEnd,
                        IsActive = !isBooked
                    });

                    start = slotEnd;
                }
            }

            return result;
        }
        public async Task<DoctorDashboardStatsDto> GetDashboardStatsAsync(string doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByUserIdAsync(doctorId);
            return new DoctorDashboardStatsDto
            {
                TodayAppointmentsCount =
                    await _unitOfWork.Appointments.GetTodayAppointmentsCountAsync(doctorId),
                TotalAppointmentCount =
                    await _unitOfWork.Appointments.GetTotalAppointmentsCountAsync(doctorId),

                YesterdayAppointmentsCount = 
                await _unitOfWork.Appointments.GetYesterdayAppointmentsCountAsync(doctorId),

                YesterdayRevenue = 
                await _unitOfWork.Appointments.GetYesterRevenueAsync(doctorId),
                TotalPatientsCount =
                    await _unitOfWork.Appointments.GetTotalPatientsCountAsync(doctorId),

                TodayRevenue =
                    await _unitOfWork.Appointments.GetTodayRevenueAsync(doctorId),

                TotalRevenue =
                    await _unitOfWork.Appointments.GetTotalRevenueAsync(doctorId),

                ReviewsCount =
                    await _unitOfWork.Reviews.GetReviewsCountAsync(doctor.Id)
            };
        }
            public async Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId)
        {
            return await _unitOfWork.Doctors.GetDoctorByUserIdReadDtoAsync(userId);
        }
        public async Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile clinicImage, IFormFile doctorImage)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("This email is already registered. Please use another one.");
            string generatedUsername = dto.UserName + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            var user = new AppUser
            {
                UserName = generatedUsername,
                Email = dto.Email,
                Role = UserRole.Doctor
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

            

            Address address;
            
            
                address = new Address
                {
                    Country = dto.ClinicAddress.Country,
                    City = dto.ClinicAddress.City,
                    Street = dto.ClinicAddress.Street,
                    PostalCode = dto.ClinicAddress.PostalCode,
                    Latitude = dto.ClinicAddress.Latitude,
                    Longitude = dto.ClinicAddress.Longitude
                };
                await _unitOfWork.Addresses.AddAsync(address);
                await _unitOfWork.CompleteAsync();
            

            string clinicImagePath = null;
            if (clinicImage != null)
            {
                clinicImagePath = await _fileStorage.SaveFileAsync(clinicImage, "clinics");
            }

            var clinic = new Clinic
            {
                Name = dto.ClinicName,
                Phone = dto.ClinicPhone,
                AddressId = address.Id,
                ImagePath = clinicImagePath
            };

            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.CompleteAsync();

            string doctorImagePath = null;
            if (doctorImage != null)
                doctorImagePath = await _fileStorage.SaveFileAsync(doctorImage, "doctors");

            var doctorProfile = new DoctorProfile
            {
                AppUserId = user.Id,
                ClinicId = clinic.Id,
                Specialty = dto.Specialty,
                ConsultationPrice = dto.ConsultationPrice ?? 0,
                ConsultationType = dto.ConsultationType,
                Gender = dto.Gender,
                IsApproved = false,
                HasPaid = false

            };

            await _unitOfWork.Doctors.AddAsync(doctorProfile);
            await _unitOfWork.CompleteAsync();

            return user;
        }

        public async Task<(bool success, string message)> ApproveDoctorAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
                return (false, "Doctor not found.");

            if (doctor.IsApproved)
                return (false, "Doctor already approved.");

            if (doctor.IsRejected)
                return (false, "Doctor was rejected before.");

            var payment = await _paymentService.GetByReferenceIdAsync(doctor.AppUserId);
            if (payment == null || string.IsNullOrEmpty(payment.PaymentIntentId))
                return (false, "Doctor has not completed the payment process.");

            var captured = await _stripePaymentService.CapturePaymentAsync(payment.PaymentIntentId);
            if (!captured)
                return (false, "Payment capture failed. Please verify payment status.");

            payment.Status = PaymentStatus.Paid;
            await _unitOfWork.CompleteAsync();

            doctor.IsApproved = true;
            doctor.IsRejected = false;
            doctor.HasPaid = true;
            await _unitOfWork.CompleteAsync();

            await _emailSender.SendEmailAsync(
                doctor.AppUser.Email,
                "Account Approved",
                $"Hello {doctor.AppUser.Email},<br/>Your account has been accepted by admin after payment confirmation."
            );

            return (true, "Doctor approved successfully after confirming payment.");
        }


        public async Task<(bool success, string message)> RejectDoctorAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
                return (false, "Doctor not found.");

            if (doctor.IsRejected)
                return (false, "Doctor already rejected.");

            if (doctor.IsApproved)
                return (false, "Doctor already approved, cannot reject.");

            doctor.IsApproved = false;
            doctor.IsRejected = true;
            doctor.HasPaid = true;
            await _unitOfWork.CompleteAsync();

            var payment = await _paymentService.GetByReferenceIdAsync(doctor.AppUserId);
            if (payment != null && !string.IsNullOrEmpty(payment.PaymentIntentId))
            {
                var canceled = await _stripePaymentService.CancelPaymentAsync(payment.PaymentIntentId);
                if (canceled)
                {
                    payment.Status = PaymentStatus.Refunded;
                    await _unitOfWork.CompleteAsync();
                }
            }

            await _emailSender.SendEmailAsync(
                doctor.AppUser.Email,
                "Account Rejected",
                $"Hello {doctor.AppUser.Email},<br/>Your account has been rejected by admin."
            );

            return (true, "Doctor rejected successfully and payment canceled.");
        }
        public async Task MarkAsPaid(string userId)
        {
            await _unitOfWork.Doctors.MarkAsPaid(userId);
            await _unitOfWork.CompleteAsync();
        }
      
        public async Task<IEnumerable<DoctorReadDto>> GetDoctorsAsync(Speciality? specialty,
    string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType)
        {
            return await _unitOfWork.Doctors.GetFilteredDoctorsAsync(specialty,name, gender, sortOrder, consultationType);
        }

        public async Task<bool> UpdateDoctorProfileAsync(
    string userId,
    DoctorUpdateDto dto)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return false;

            var user = doctor.AppUser;
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
                if (!emailResult.Succeeded)
                    return false;

                var userNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
                if (!userNameResult.Succeeded)
                    return false;
            }

            if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
            {
                user.UserName = dto.UserName;
                user.NormalizedUserName = dto.UserName.ToUpper();
            }

            if (dto.Specialty.HasValue)
                doctor.Specialty = dto.Specialty.Value;

            if (dto.Gender.HasValue)
                doctor.Gender = dto.Gender.Value;

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.CompleteAsync();

            return true;
        }



        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var clinic = await _unitOfWork.Clinics.GetByIdAsync(doctor.ClinicId ?? 0);
            if (clinic != null)
            {
                if (!string.IsNullOrEmpty(clinic.ImagePath))
                    _fileStorage.DeleteFile(clinic.ImagePath, "clinics");

                _unitOfWork.Clinics.Delete(clinic);
            }

            _unitOfWork.Doctors.Delete(doctor);

            if (!string.IsNullOrEmpty(doctor.AppUserId))
            {
                var user = await _userManager.FindByIdAsync(doctor.AppUserId);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                        throw new Exception("Failed to delete user account.");
                }
            }
            var payment = await _paymentService.GetByReferenceIdAsync(doctor.AppUserId);
            if (payment != null)
            {
                _paymentService.DeletePaymentAsync(payment);
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task<DoctorReadDto?> GetByIdDetailsAsync(int id)
        {
            return await _unitOfWork.Doctors.GetByIdDetailsAsync(id);
        }

        public async Task<DoctorProfile> GetDoctorByIdAsync(int id)
        {
            return await _unitOfWork.Doctors.GetByIdAsync(id);
        }

        public async Task<DoctorProfile?> GetDoctorDetailsByUserIdAsync(string userId)
        {
            return await _unitOfWork.Doctors.GetDoctorByUserIdAsync(userId);
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync()
        {
            return await _unitOfWork.Doctors.GetAllDoctorsAcceptedByAdminAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdmin()
        {
            return await _unitOfWork.Doctors.GetAllDoctorsShowToAdminAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetTopRatedDoctorsAsync(3);
            return doctors;
        }
    }

}
