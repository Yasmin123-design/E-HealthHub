using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<Medication>> GetAllMedicinesAsync();
        Task<Medication> GetMedicineByIdAsync(int id);
        Task AddMedicineAsync(Medication medicine, IFormFile? imageFile);
        Task UpdateMedicineAsync(int id ,Medication medicine, IFormFile? imageFile);
        Task DeleteMedicineAsync(int id);
        Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId);

        Task<IEnumerable<Medication>> SearchMedicinesByNameAsync(string name);
        Task<IEnumerable<Pharmacy>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng);
    }
}
