using E_PharmaHub.Dtos;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class DoctorFavouriteService : IDoctorFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorFavouriteService(IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int doctorId)
        {
            var added = await _unitOfWork.FavouriteDoctor.AddToFavoritesAsync(userId, doctorId);
            if (!added)
                return false;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int doctorId)
        {
            var removed = await _unitOfWork.FavouriteDoctor.RemoveFromFavoritesAsync(userId, doctorId);
            if (!removed)
                return false;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorReadDto>> GetUserFavoritesAsync(string userId)
        {
            var favs = await _unitOfWork.FavouriteDoctor.GetUserFavoritesAsync(userId);
            return favs;
        }
    }
}
