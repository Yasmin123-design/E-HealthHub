using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;
using Address = E_PharmaHub.Models.Address;

namespace E_PharmaHub.Services
{
    public class PharmacistService : IPharmacistService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileStorageService _fileStorage;
        private readonly IStripePaymentService _stripePaymentService;
        private readonly IPaymentService _paymentService;
        private readonly IEmailSender _emailSender;


        private readonly IUnitOfWork _unitOfWork;

        public PharmacistService(UserManager<AppUser> userManager,
            IFileStorageService fileStorage,
            IUnitOfWork unitOfWork,
            IStripePaymentService stripePaymentService,
            IPaymentService paymentService,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _stripePaymentService = stripePaymentService;
            _paymentService = paymentService;
            _emailSender = emailSender;
        }

        public async Task<AppUser> RegisterPharmacistAsync(PharmacistRegisterDto dto, IFormFile pharmacyImage, IFormFile pharmacistImage)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("This email is already registered. Please use another one.");

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Role = UserRole.Pharmacist,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, UserRole.Pharmacist.ToString());

            var existingAddress = await _unitOfWork.Addresses.FindAsync(a =>
                a.Country == dto.Address.Country &&
                a.City == dto.Address.City &&
                a.Street == dto.Address.Street &&
                a.PostalCode == dto.Address.PostalCode &&
                a.Latitude == dto.Address.Latitude &&
                a.Longitude == dto.Address.Longitude
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
                    Country = dto.Address.Country,
                    City = dto.Address.City,
                    Street = dto.Address.Street,
                    PostalCode = dto.Address.PostalCode,
                    Latitude = dto.Address.Latitude,
                    Longitude = dto.Address.Longitude
                };
                await _unitOfWork.Addresses.AddAsync(address);
                await _unitOfWork.CompleteAsync();
            }

            string? pharmacyImagePath = null;
            if (pharmacyImage != null && pharmacyImage.Length > 0)
            {
                pharmacyImagePath = await _fileStorage.SaveFileAsync(pharmacyImage, "pharmacies");
            }

            var pharmacy = new Pharmacy
            {
                Name = dto.PharmacyName,
                Phone = dto.PhoneNumber,
                AddressId = address.Id,
                ImagePath = pharmacyImagePath
            };
            await _unitOfWork.Pharmacies.AddAsync(pharmacy);
            await _unitOfWork.CompleteAsync();

            string? pharmacistImagePath = null;
            if (pharmacistImage != null && pharmacistImage.Length > 0)
            {
                pharmacistImagePath = await _fileStorage.SaveFileAsync(pharmacistImage, "pharmacistes");
            }
            var pharmacistProfile = new PharmacistProfile
            {
                AppUserId = user.Id,
                PharmacyId = pharmacy.Id,
                LicenseNumber = dto.LicenseNumber,
                Image = pharmacistImagePath,
                IsApproved = false,
                HasPaid = false  

            };

            await _unitOfWork.PharmasistsProfile.AddAsync(pharmacistProfile);
            await _unitOfWork.CompleteAsync();

            return user;
        }

        public async Task AddPharmacistAsync(PharmacistProfile pharmacist)
        {
            await _unitOfWork.PharmasistsProfile.AddAsync(pharmacist);
            await _unitOfWork.CompleteAsync();
        }
        public async Task MarkAsPaid(string userId)
        {
            await _unitOfWork.PharmasistsProfile.MarkAsPaid(userId);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<PharmacistReadDto?> GetPharmacistByUserIdAsync(string userId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetPharmacistReadDtoByUserIdAsync(userId);
            if (pharmacist == null)
                return null;

            return pharmacist;
        }

        public async Task<(bool success, string message)> ApprovePharmacistAsync(int pharmacistId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(pharmacistId);
            if (pharmacist == null)
                return (false, "Pharmacist not found.");

            if (pharmacist.IsApproved)
                return (false, "Pharmacist already approved.");

            if (pharmacist.IsRejected)
                return (false, "Pharmacist was rejected before.");

            var payment = await _paymentService.GetByReferenceIdAsync(pharmacist.AppUserId);
            if (payment == null || string.IsNullOrEmpty(payment.PaymentIntentId))
                return (false, "Pharmacist has not completed the payment process.");

            var captured = await _stripePaymentService.CapturePaymentAsync(payment.PaymentIntentId);
            if (!captured)
                return (false, "Payment capture failed. Please verify payment status.");

            payment.Status = PaymentStatus.Paid;
            await _unitOfWork.CompleteAsync();

            pharmacist.IsApproved = true;
            pharmacist.IsRejected = false;
            pharmacist.HasPaid = true;
            await _unitOfWork.CompleteAsync();

            await _emailSender.SendEmailAsync(
                pharmacist.AppUser.Email,
                "Account Approved",
                $"Hello {pharmacist.AppUser.Email},<br/>Your pharmacist account has been accepted by admin after payment confirmation."
            );

            return (true, "Pharmacist approved successfully after confirming payment.");
        }


        public async Task<(bool success, string message)> RejectPharmacistAsync(int pharmacistId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(pharmacistId);
            if (pharmacist == null)
                return (false, "Pharmacist not found.");

            if (pharmacist.IsRejected)
                return (false, "Pharmacist already rejected.");

            if (pharmacist.IsApproved)
                return (false, "Pharmacist already approved, cannot reject.");

            pharmacist.IsApproved = false;
            pharmacist.IsRejected = true;
            await _unitOfWork.CompleteAsync();

            var payment = await _paymentService.GetByReferenceIdAsync(pharmacist.AppUserId);
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
                pharmacist.AppUser.Email,
                "Account Rejected",
                $"Hello {pharmacist.AppUser.Email},<br/>We regret to inform you that your pharmacist account was rejected by admin."
            );

            return (true, "Pharmacist rejected successfully and payment refunded.");
        }

        public async Task<bool> UpdatePharmacistProfileAsync(string userId, PharmacistUpdateDto dto, IFormFile? image)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByUserIdAsync(userId);
            if (pharmacist == null)
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.UserName))
                user.UserName = dto.UserName;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(dto.CurrentPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passResult.Succeeded)
                {
                    var errors = string.Join(", ", passResult.Errors.Select(e => e.Description));
                    throw new Exception($"Password update failed: {errors}");
                }
            }

            if (!string.IsNullOrEmpty(dto.LicenseNumber))
                pharmacist.LicenseNumber = dto.LicenseNumber;

            if (image != null)
            {
                var imagePath = await _fileStorage.SaveFileAsync(image, "pharmacistes");
                pharmacist.Image = imagePath;
            }

            _unitOfWork.PharmasistsProfile.Update(pharmacist);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task DeletePharmacistAsync(int id)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found.");

            var user = await _userManager.FindByIdAsync(pharmacist.AppUserId);
            if (user != null)
                await _userManager.DeleteAsync(user);

            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacist.PharmacyId);
            if (pharmacy != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.ImagePath))
                    _fileStorage.DeleteFile(pharmacy.ImagePath);

                _unitOfWork.Pharmacies.Delete(pharmacy);
            }

            _unitOfWork.PharmasistsProfile.Delete(pharmacist);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<PharmacistReadDto>> GetAllPharmacistsAsync()
        {
            return await _unitOfWork.PharmasistsProfile.GetAllDetailsAsync();
        }

        public async Task<PharmacistReadDto?> GetPharmacistByIdAsync(int id)
        {
            return await _unitOfWork.PharmasistsProfile.GetByIdDetailsAsync(id);
        }
        public async Task<PharmacistProfile?> GetPharmacistProfileByIdAsync(int id)
        {
            return await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
        }

        public async Task<PharmacistProfile?> GetPharmacistProfileByUserIdAsync(string userId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByUserIdAsync(userId);
            if (pharmacist == null)
                return null;
            return pharmacist;
        }
    }



}


