using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.FavoriteMedicationRepo
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
            var exists = await _context.FavoriteMedications.AsNoTracking()
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
            var fav = await _context.FavoriteMedications.AsNoTracking()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.MedicationId == medicationId);

            if (fav == null) return false;

            _context.FavoriteMedications.Remove(fav);
            return true;
        }

        public async Task<IEnumerable<MedicineDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _context.FavoriteMedications.AsNoTracking()
                .Where(f => f.UserId == userId)
                .Include(f => f.Medication)
                 .ThenInclude(m => m.Inventories)

                        .ThenInclude(i => i.Pharmacy)
                            .ThenInclude(p => p.Address)
                .Include(f => f.Medication)
            .ThenInclude(m => m.Reviews)
                .ToListAsync();

            var result = new List<MedicineDto>();

            foreach (var fav in favorites)
            {
                var inventoryItem = fav.Medication.Inventories
                    .OrderBy(i => i.Price)
                    .FirstOrDefault();

                if (inventoryItem != null)
                {
                    result.Add(MedicineSelector.MapInventoryToDto(inventoryItem));
                }
            }

            return result;
        }


    }
}
