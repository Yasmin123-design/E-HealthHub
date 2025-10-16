using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly EHealthDbContext _context;

        public MedicineRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await _context.Medications
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                .ToListAsync();
        }
        public async Task<Medication?> FindAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _context.Medications.FirstOrDefaultAsync(predicate);
        }
        public async Task<Medication> GetByIdAsync(int id)
        {
            return await _context.Medications
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Medication entity)
        {
            await _context.Medications.AddAsync(entity);
        }

        public async Task Update(Medication entity)
        {
            _context.Medications.Update(entity);
        }

        public void Delete(Medication entity)
        {
            _context.Medications.Remove(entity);
        }

        public async Task<IEnumerable<Medication>> SearchByNameAsync(string name)
        {
            return await _context.Medications
                .Include(x => x.Inventories)
                .ThenInclude(x => x.Pharmacy)
                .Where(m => m.BrandName.Contains(name) || m.GenericName.Contains(name))
                .ToListAsync();
        }
        public async Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .Include(i => i.Medication)
                .Where(i => i.PharmacyId == pharmacyId)
                .Select(i => i.Medication)
                .ToListAsync();
        }

        public async Task<IEnumerable<NearestPharmacyDTO>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng)
        {
            var pharmacies = await _context.Pharmacies
                .Include(p => p.Address)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Medication)
                .Where(p => p.Inventory.Any(i =>
                    i.Medication.BrandName.Contains(medicationName) ||
                    i.Medication.GenericName.Contains(medicationName)))
                .ToListAsync();

            // Haversine Formula
            var result = pharmacies.Select(p =>
            {
                var medication = p.Inventory.FirstOrDefault(i =>
                    i.Medication.BrandName.Contains(medicationName) ||
                    i.Medication.GenericName.Contains(medicationName))?.Medication;

                var dLat = (p.Address.Latitude ?? 0 - userLat) * Math.PI / 180;
                var dLng = (p.Address.Longitude ?? 0 - userLng) * Math.PI / 180;
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(userLat * Math.PI / 180) * Math.Cos((p.Address.Latitude ?? 0) * Math.PI / 180) *
                        Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var distance = 6371 * c;

                return new NearestPharmacyDTO
                {
                    PharmacyName = p.Name,
                    City = p.Address?.City,
                    Street = p.Address?.Street,
                    Phone = p.Phone,
                    MedicationName = medication?.BrandName ?? medicationName,
                    DistanceKm = Math.Round(distance, 2)
                };
            });

            return result.OrderBy(r => r.DistanceKm);
        }

    }

}
