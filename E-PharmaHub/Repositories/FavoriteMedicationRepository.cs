using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class FavoriteMedicationRepository : IFavoriteMedicationRepository
    {
        private readonly EHealthDbContext _context;

        public FavoriteMedicationRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int medicationId)
        {
            var exists = await _context.FavoriteMedications
                .AnyAsync(f => f.UserId == userId && f.MedicationId == medicationId);

            if (exists) return false;

            await _context.FavoriteMedications.AddAsync(new FavoriteMedication
            {
                UserId = userId,
                MedicationId = medicationId
            });

            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int medicationId)
        {
            var fav = await _context.FavoriteMedications
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MedicationId == medicationId);

            if (fav == null) return false;

            _context.FavoriteMedications.Remove(fav);
            return true;
        }

        public async Task<IEnumerable<object>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _context.FavoriteMedications
                .Where(f => f.UserId == userId)
                .Include(f => f.Medication)
                    .ThenInclude(m => m.Inventories)
                .Select(f => new
                {
                    f.Medication.Id,
                    f.Medication.BrandName,
                    f.Medication.GenericName,
                    f.Medication.DosageForm,
                    f.Medication.Strength,
                    f.Medication.ATCCode,
                    f.Medication.ImagePath,

                    Price = f.Medication.Inventories != null && f.Medication.Inventories.Any()
                        ? f.Medication.Inventories.Min(i => i.Price)
                        : 0
                })
                .ToListAsync();

            return favorites;
        }

    }
}
