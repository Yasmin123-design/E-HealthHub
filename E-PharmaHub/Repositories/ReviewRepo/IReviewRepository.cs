using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.ReviewRepo
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<int> GetReviewsCountAsync(int doctorId);
        Task<IEnumerable<ReviewDto>> GetReviewDtosByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<ReviewDto>> GetReviewDtosByMedicationIdAsync(int medicationId);
        Task<IEnumerable<ReviewDto>> GetReviewDtosByDoctorIdAsync(int doctorId);


    }

}
