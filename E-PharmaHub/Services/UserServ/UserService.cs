using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories.UserRepo;
using E_PharmaHub.Services.FileStorageServ;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services.UserServ
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileStorageService _fileStorage;

        public UserService(IUserRepository userRepo, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IFileStorageService fileStorage)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileStorage = fileStorage;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return null;

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Address = user.Address
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UserProfileDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                var emailResult = await _userManager.SetEmailAsync(user, dto.Email);
                if (!emailResult.Succeeded)
                    return (false, "Failed to update email ❌");

                var userNameResult = await _userManager.SetUserNameAsync(user, dto.Email);
                if (!userNameResult.Succeeded)
                    return (false, "Failed to update username ❌");
            }

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                user.UserName = dto.UserName;
                user.NormalizedUserName = dto.UserName.ToUpper();
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.Address))
                user.Address = dto.Address;

            _userRepo.Update(user);
            await _unitOfWork.CompleteAsync();
            return (true, "Profile updated successfully ✅");
        }



        public async Task<(bool Success, string Message)> UpdatePasswordAsync(string userId, UserPasswordUpdateDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var passResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!passResult.Succeeded)
                return (false, "Incorrect current password ❌");

            return (true, "Password updated successfully 🔐");
        }

        public async Task<(bool Success, string Message)> UploadOrUpdateProfilePictureAsync(string userId, IFormFile image)
        {
            if (image == null || image.Length == 0)
                return (false, "Please upload a valid image 🖼️⚠️");

            var user = await _unitOfWork.Useres.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found ❌");

            if (!string.IsNullOrEmpty(user.ProfileImage))
                _fileStorage.DeleteFile(user.ProfileImage, "users");

            var imagePath = await _fileStorage.SaveFileAsync(image, "users");
            user.ProfileImage = imagePath;

            _unitOfWork.Useres.Update(user);
            await _unitOfWork.CompleteAsync();

            return (true, "Profile picture uploaded/updated successfully 🖼️✅");
        }


        public async Task<(bool Success, string Message)> DeleteAccountAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            await _userManager.DeleteAsync(user);
            await _unitOfWork.CompleteAsync();
            return (true, "Account deleted successfully 🗑️");
        }
    }
}

