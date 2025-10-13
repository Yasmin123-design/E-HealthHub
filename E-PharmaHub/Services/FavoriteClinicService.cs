using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class FavoriteClinicService : IFavoriteClinicService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteClinicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int clinicId)
        {
            var added =  await _unitOfWork.FavouriteClinic.AddToFavoritesAsync(userId, clinicId);
            if (!added)
                return false;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int clinicId)
        {
            var removed = await _unitOfWork.FavouriteClinic.RemoveFromFavoritesAsync(userId, clinicId);
            if (!removed)
                return false;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<object>> GetUserFavoritesAsync(string userId)
        {
            return await _unitOfWork.FavouriteClinic.GetUserFavoritesAsync(userId);
        }
    }

}
