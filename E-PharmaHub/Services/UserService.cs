using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services
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
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Address = user.Address
            };
        }

        public async Task<(bool Success, string Message)> UpdateUserProfileAsync(string userId, UserUpdateDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.CurrentPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                var passResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!passResult.Succeeded)
                    return (false, "Incorrect current password ❌");
            }

            _userRepo.Update(user);
            await _unitOfWork.CompleteAsync();
            return (true, "Profile updated successfully ✅");
        }

        public async Task<(bool Success, string Message)> UploadProfilePictureAsync(string userId, IFormFile image)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            var imagePath = await _fileStorage.SaveFileAsync(image, "users");
            user.ProfileImage = imagePath;

            _userRepo.Update(user);
            await _unitOfWork.CompleteAsync();
            return (true, "Profile picture updated successfully 🖼️✅");
        }

        public async Task<(bool Success, string Message)> UpdateProfilePictureAsync(string userId, IFormFile newImage)
        {
            var user = await _unitOfWork.Useres.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found ❌");

            if (newImage == null || newImage.Length == 0)
                return (false, "Please upload a valid image 🖼️⚠️");

            if (!string.IsNullOrEmpty(user.ProfileImage))
                 _fileStorage.DeleteFile(user.ProfileImage);

            var newPath = await _fileStorage.SaveFileAsync(newImage, "users");
            user.ProfileImage = newPath;

            _unitOfWork.Useres.Update(user);
            await _unitOfWork.CompleteAsync();

            return (true, "Profile picture updated successfully 🖼️✅");
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

