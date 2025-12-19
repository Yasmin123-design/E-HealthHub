using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories.PrescriptionItemRepo
{
    public class PrescriptionItemRepository : IPrescriptionItemRepository
    {
        private readonly EHealthDbContext _context;

        public PrescriptionItemRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<PrescriptionItem?> GetByIdAsync(int id)
        {
            return await _context.PrescriptionItems
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<PrescriptionItem>> GetByPrescriptionIdAsync(int prescriptionId)
        {
            return await _context.PrescriptionItems
                .Where(i => i.PrescriptionId == prescriptionId)
                .ToListAsync();
        }

        public async Task AddAsync(PrescriptionItem item)
        {
            await _context.PrescriptionItems.AddAsync(item);
        }

        public void Update(PrescriptionItem item)
        {
            _context.PrescriptionItems.Update(item);
        }

        public void Delete(PrescriptionItem item)
        {
            _context.PrescriptionItems.Remove(item);
        }
    }

}
