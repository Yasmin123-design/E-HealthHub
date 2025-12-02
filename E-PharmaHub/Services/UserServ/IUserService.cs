using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.UserServ
{
    public interface IUserService
    {
        Task<(bool Success, string Message)> UpdatePasswordAsync(string userId, UserPasswordUpdateDto dto);

        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UserProfileDto dto);
        Task<(bool Success, string Message)> DeleteAccountAsync(string userId);
        Task<(bool Success, string Message)> UploadOrUpdateProfilePictureAsync(string userId, IFormFile image);
    }
}
