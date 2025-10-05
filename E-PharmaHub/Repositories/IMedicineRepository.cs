using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IMedicineRepository : IGenericRepository<Medication>
    {
        Task<IEnumerable<Medication>> SearchByNameAsync(string name);
        Task<IEnumerable<Pharmacy>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng);
        Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId);

    }
}
