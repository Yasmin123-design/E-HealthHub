namespace E_PharmaHub.Services.FavoriteClinicServ
{
    public interface IFavoriteClinicService
    {
        Task<bool> AddToFavoritesAsync(string userId, int clinicId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int clinicId);
        Task<IEnumerable<object>> GetUserFavoritesAsync(string userId);
    }
}
