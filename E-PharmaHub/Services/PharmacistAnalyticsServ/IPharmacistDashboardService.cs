using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.PharmacistAnalyticsServ
{
    public interface IPharmacistDashboardService
    {
        Task<WeeklyCategoryItemsDto> GetWeeklyCategoryItemsAsync(string userId);
        Task<WeeklyOrdersDashboardDto> GetWeeklyOrdersAsync(string userId);
        Task<InventoryDashboardDto> GetInventoryDashboardAsync(string userId);

    }
}
