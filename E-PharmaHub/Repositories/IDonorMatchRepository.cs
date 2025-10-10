using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IDonorMatchRepository
    {
        Task<IEnumerable<DonorMatch>> MatchDonorsForRequestAsync(int requestId);

    }
}
