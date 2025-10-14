using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<(bool Success, string Message)> UpdateUserProfileAsync(string userId, UserUpdateDto dto);
        Task<(bool Success, string Message)> UploadProfilePictureAsync(string userId, IFormFile image);
        Task<(bool Success, string Message)> DeleteAccountAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfilePictureAsync(string userId, IFormFile newImage);
    }
}
