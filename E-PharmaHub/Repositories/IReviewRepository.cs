using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<ReviewDto>> GetReviewDtosByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<ReviewDto>> GetReviewDtosByMedicationIdAsync(int medicationId);
        Task<IEnumerable<ReviewDto>> GetReviewDtosByDoctorIdAsync(int doctorId);


    }

}
