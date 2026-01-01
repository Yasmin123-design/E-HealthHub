using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class CategoryItemsCountDto
    {
        public MedicationCategory CategoryName { get; set; }
        public int ItemsCount { get; set; }
    }
}
