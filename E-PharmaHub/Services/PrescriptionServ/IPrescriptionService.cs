using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.PrescriptionServ
{
    public interface IPrescriptionService
    {
        Task<List<PrescriptionDto>> GetUserPrescriptionsAsync(string userId);
        Task CreateAsync(CreatePrescriptionDto dto);
        Task AddItemAsync(int prescriptionId, PrescriptionItemDto dto);
        Task UpdateItemAsync(int itemId, PrescriptionItemDto dto);
        Task DeleteItemAsync(int itemId);
        Task UpdateItemsAsync(int prescriptionId, List<PrescriptionItemDto> items);
        Task DeleteAsync(int prescriptionId);
    }
}
