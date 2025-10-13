namespace E_PharmaHub.Repositories
{
    public interface IFavouriteClinicRepository
    {
        Task<bool> AddToFavoritesAsync(string userId, int clinicId);
        Task<bool> RemoveFromFavoritesAsync(string userId, int clinicId);
        Task<IEnumerable<object>> GetUserFavoritesAsync(string userId);

    }
}
