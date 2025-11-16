using E_PharmaHub.Dtos;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class FavoriteMedicationService : IFavoriteMedicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteMedicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int medicationId)
        {
            var added = await _unitOfWork.Favorite.AddToFavoritesAsync(userId, medicationId);
            if (!added)
                return false;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int medicationId)
        {
            var removed = await _unitOfWork.Favorite.RemoveFromFavoritesAsync(userId, medicationId);
            if (!removed)
                return false;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<MedicineDto>> GetUserFavoritesAsync(string userId)
        {
            var favs = await _unitOfWork.Favorite.GetUserFavoritesAsync(userId);
            return favs;
        }
    }
}
