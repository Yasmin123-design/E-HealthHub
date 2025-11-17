using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewByIdAsync(int id);
        Task AddReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(int id, Review updatedReview, string userId);
        Task<bool> DeleteReviewAsync(int id, string userId);
        Task<IEnumerable<ReviewDto>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<ReviewDto>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<IEnumerable<ReviewDto>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<double> GetAverageRatingForPharmacyAsync(int pharmacyId);
        Task<double> GetAverageRatingForMedicationAsync(int medicationId);
        Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync();
        Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync();
        Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync();

    }
}
