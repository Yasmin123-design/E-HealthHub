using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message)> UpdatePasswordAsync(string userId, UserPasswordUpdateDto dto);

        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UserProfileDto dto);
        Task<(bool Success, string Message)> UploadProfilePictureAsync(string userId, IFormFile image);
        Task<(bool Success, string Message)> DeleteAccountAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfilePictureAsync(string userId, IFormFile newImage);
    }
}
