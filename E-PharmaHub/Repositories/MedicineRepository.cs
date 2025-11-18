using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Helpers;
using Microsoft.EntityFrameworkCore;
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

        private IQueryable<Medication> BaseMedicationIncludes()
        {
            return _context.Medications
                .AsNoTracking()
                .Include(m => m.Reviews)
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                        .ThenInclude(p => p.Address);
        }

        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await BaseMedicationIncludes().ToListAsync();
        }

        public async Task<Medication?> FindAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await BaseMedicationIncludes().FirstOrDefaultAsync(predicate);
        }

        public async Task<Medication> GetByIdAsync(int id)
        {
            var medicine = await BaseMedicationIncludes()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null || !medicine.Inventories.Any())
                return null;

            return medicine;
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

        public async Task<IEnumerable<MedicineDto>> SearchByNameAsync(string name)
        {
            var meds = await BaseMedicationIncludes()
                .Where(m => m.BrandName.Contains(name) || m.GenericName.Contains(name))
                .ToListAsync();

            var dtos = meds
               .SelectMany(m => m.Inventories
                   .Select(inv => MedicineSelector.MapInventoryToDto(inv)))
               .ToList();
            return dtos;
        }

        public async Task<IEnumerable<MedicineDto>> GetMedicinesByPharmacyIdAsync(int pharmacyId)
        {
            var meds = await BaseMedicationIncludes()
                .Where(m => m.Inventories.Any(inv => inv.PharmacyId == pharmacyId))
                .ToListAsync();

            var dtos = meds
                .SelectMany(m => m.Inventories
                    .Where(inv => inv.PharmacyId == pharmacyId)
                    .Select(inv => MedicineSelector.MapInventoryToDto(inv)))
                .ToList();

            return dtos;
        }


    }
}

