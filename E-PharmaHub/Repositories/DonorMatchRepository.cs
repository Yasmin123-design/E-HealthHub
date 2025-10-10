using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class DonorMatchRepository : IDonorMatchRepository
    {
        private readonly EHealthDbContext _context;

        public DonorMatchRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DonorMatch>> MatchDonorsForRequestAsync(int requestId)
        {
            var request = await _context.BloodRequests.FindAsync(requestId);
            if (request == null)
                throw new Exception("Request not found.");

            var matchingDonors = await _context.DonorProfiles
                .Where(d => d.BloodType == request.RequiredType
                            && d.City.ToLower() == request.City.ToLower()
                            && d.IsAvailable)
                .ToListAsync();

            var matches = matchingDonors.Select(d => new DonorMatch
            {
                BloodRequestId = request.Id,
                DonorUserId = d.AppUserId,
                Notified = false
            }).ToList();

            await _context.DonorMatches.AddRangeAsync(matches);

            return matches;
        }
    }
}

