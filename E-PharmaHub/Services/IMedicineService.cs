using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IMedicineService
    {
        Task<IEnumerable<Medication>> GetAllMedicinesAsync();
        Task<Medication> GetMedicineByIdAsync(int id);
        Task UpdateMedicineAsync(int id, MedicineDto dto, IFormFile? image, int? pharmacyId);
        Task DeleteMedicineAsync(int id, int? pharmacyId);
        Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId);

        Task AddMedicineWithInventoryAsync(MedicineDto dto, IFormFile? image, int pharmacyId);
        Task<IEnumerable<Medication>> SearchMedicinesByNameAsync(string name);
        Task<IEnumerable<Pharmacy>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng);
    }
}
