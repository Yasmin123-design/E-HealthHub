using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services
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
        public async Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId)
        {
            return await _unitOfWork.Doctors.GetDoctorByUserIdReadDtoAsync(userId);
        }
        public async Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile clinicImage,IFormFile doctorImage)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("This email is already registered. Please use another one.");

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = UserRole.Doctor
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

            var existingAddress = await _unitOfWork.Addresses.FindAsync(a =>
                a.Country == dto.ClinicAddress.Country &&
                a.City == dto.ClinicAddress.City &&
                a.Street == dto.ClinicAddress.Street &&
                a.PostalCode == dto.ClinicAddress.PostalCode &&
                a.Latitude == dto.ClinicAddress.Latitude &&
                a.Longitude == dto.ClinicAddress.Longitude
            );

            Address address;
            if (existingAddress != null)
            {
                address = existingAddress;
            }
            else
            {
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
            }

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
                ConsultationPrice = dto.ConsultationPrice??0,
                ConsultationType = dto.ConsultationType,
                Gender = dto.Gender,
                IsApproved = false,
                HasPaid = false,
                Image = doctorImagePath

            };

            await _unitOfWork.Doctors.AddAsync(doctorProfile);
            await _unitOfWork.CompleteAsync();

            return user;
        }


        public async Task<(bool success, string message)> ApproveDoctorAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
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
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(doctorId);
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
        public async Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            return await _unitOfWork.Doctors.GetDoctorsBySpecialtyAsync(specialty);
        }
        public async Task<IEnumerable<DoctorReadDto>> GetDoctorsAsync(
    string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType)
        {
            return await _unitOfWork.Doctors.GetFilteredDoctorsAsync(name, gender, sortOrder, consultationType);
        }

        public async Task<int?> GetDoctorPatientCountAsync(string doctorId)
        {
            var doctorExists = await _unitOfWork.Doctors.GetDoctorByUserIdAsync(doctorId);
            if (doctorExists == null)
                return null;

            var count = await _unitOfWork.Doctors.GetDoctorPatientCountAsync(doctorId);
            return count;
        }

        public async Task<int?> GetDoctorReviewCountAsync(int doctorProfileId)
        {
            var doctorProfileExists = await _unitOfWork.Doctors.GetByIdAsync(doctorProfileId);
            if (doctorProfileExists == null)
                return null;

            var count = await _unitOfWork.Doctors.GetDoctorReviewCountAsync(doctorProfileId);
            return count;
        }
        public async Task<bool> UpdateDoctorProfileAsync(string userId, DoctorUpdateDto dto, IFormFile? doctorImage)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(
                (await _unitOfWork.Doctors.GetDoctorByUserIdAsync(userId))?.Id ?? 0
            );

            if (doctor == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Specialty))
                doctor.Specialty = dto.Specialty;

            if (dto.ConsultationPrice != 0)
                doctor.ConsultationPrice = dto.ConsultationPrice;


            doctor.Gender = dto.Gender;
            doctor.ConsultationType = dto.ConsultationType;



            if (doctorImage != null)
            {
                if (!string.IsNullOrEmpty(doctor.Image))
                    _fileStorage.DeleteFile(doctor.Image);

                doctor.Image = await _fileStorage.SaveFileAsync(doctorImage, "doctors");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User account not found.");

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);
                var emailResult = await _userManager.ChangeEmailAsync(user, dto.Email, token);

                if (!emailResult.Succeeded)
                    throw new Exception(string.Join(", ", emailResult.Errors.Select(e => e.Description)));

                user.Email = dto.Email;
                await _userManager.UpdateAsync(user);
            }

            if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
            {
                user.UserName = dto.UserName;
                await _userManager.UpdateAsync(user);
            }

            if (!string.IsNullOrEmpty(dto.CurrentPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passResult.Succeeded)
                    throw new Exception(string.Join(", ", passResult.Errors.Select(e => e.Description)));
            }

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.CompleteAsync();

            return true;
        }


        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found.");
            
            var clinic = await _unitOfWork.Clinics.GetClinicByIdAsync(doctor.ClinicId);
            if (clinic != null)
            {
                if (!string.IsNullOrEmpty(clinic.ImagePath))
                    _fileStorage.DeleteFile(clinic.ImagePath);

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
            if(payment != null)
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
    }

}
