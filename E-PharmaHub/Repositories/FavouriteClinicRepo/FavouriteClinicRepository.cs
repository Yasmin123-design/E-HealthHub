using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.FavouriteClinicRepo
{
    public class FavouriteClinicRepository : IFavouriteClinicRepository
    {
        private readonly EHealthDbContext _context;

        public FavouriteClinicRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int clinicId)
        {
            var exists = await _context.FavoriteClinics
                .AnyAsync(f => f.UserId == userId && f.ClinicId == clinicId);

            if (exists) return false;

            var favorite = new FavoriteClinic
            {
                UserId = userId,
                ClinicId = clinicId
            };

            await _context.FavoriteClinics.AddAsync(favorite);
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int clinicId)
        {
            var favorite = await _context.FavoriteClinics
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ClinicId == clinicId);

            if (favorite == null) return false;

            _context.FavoriteClinics.Remove(favorite);
            return true;
        }

        public async Task<IEnumerable<object>> GetUserFavoritesAsync(string userId)
        {
            return await _context.FavoriteClinics
                .Where(f => f.UserId == userId)
                .Include(f => f.Clinic)
                .Select(f => new
                {
                    f.Clinic.Id,
                    f.Clinic.Name,
                    f.Clinic.Phone,
                    f.Clinic.Address,
                    f.Clinic.ImagePath
                })
                .ToListAsync();
        }
    }
}
