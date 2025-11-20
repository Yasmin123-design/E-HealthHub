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
        Task<IEnumerable<MedicineDto>> GetMedicinesByPharmacyIdAsync(int pharmacyId);

        Task<(bool Success, string Message)> AddMedicineWithInventoryAsync(MedicineDto dto, IFormFile? image, int pharmacyId);
        Task<IEnumerable<MedicineDto>> SearchMedicinesByNameAsync(string name);
        Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync();
    }
}
