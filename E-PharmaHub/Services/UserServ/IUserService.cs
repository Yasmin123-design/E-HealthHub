using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.UserServ
{
    public interface IUserService
    {
        Task RevokeAllRefreshTokensAsync(string userId);
        Task SaveRefreshTokenAsync(string userId, string refreshToken);
        Task<RefreshToken?> RotateRefreshTokenAsync(string oldRefreshToken);
        Task<(bool Success, string Message)> UpdatePasswordAsync(string userId, UserPasswordUpdateDto dto);
        Task<UpdateLocationResult> UpdateUserLocationAsync(
        string userId, double lat, double lng);
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UserProfileDto dto);
        Task<(bool Success, string Message)> DeleteAccountAsync(string userId);
        Task<(bool Success, string Message)> UploadOrUpdateProfilePictureAsync(string userId, IFormFile image);
    }
}
