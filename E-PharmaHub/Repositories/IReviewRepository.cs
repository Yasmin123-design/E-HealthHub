using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<double> GetAverageRatingForPharmacyAsync(int pharmacyId);
        Task<double> GetAverageRatingForMedicationAsync(int medicationId);
        Task<IEnumerable<Pharmacy>> GetTopRatedPharmaciesAsync(int count);
        Task<IEnumerable<DoctorProfile>> GetTopRatedDoctorsAsync(int count);
        Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync(int count);

    }

}
