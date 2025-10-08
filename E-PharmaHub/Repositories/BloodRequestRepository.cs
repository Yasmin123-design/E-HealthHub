using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class BloodRequestRepository : IBloodRequestRepository
    {
        private readonly EHealthDbContext _context;

        public BloodRequestRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BloodRequest>> GetAllAsync()
        {
            return await _context.BloodRequests
                .Include(r => r.Matches)
                .Include(r => r.RequestedBy)
                .ToListAsync();
        }

        public async Task<BloodRequest?> GetByIdAsync(int id)
        {
            return await _context.BloodRequests
                .Include(r => r.Matches)
                .Include(r => r.RequestedBy)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(BloodRequest entity)
        {
            await _context.BloodRequests.AddAsync(entity);
        }

        public void Update(BloodRequest entity)
        {
            _context.BloodRequests.Update(entity);
        }

        public void Delete(BloodRequest entity)
        {
            _context.BloodRequests.Remove(entity);
        }

        public async Task<IEnumerable<BloodRequest>> GetUnfulfilledRequestsAsync()
        {
            return await _context.BloodRequests
                .Include(r => r.Matches)
                .Include(r => r.RequestedBy)
                .Where(r => !r.Fulfilled)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
