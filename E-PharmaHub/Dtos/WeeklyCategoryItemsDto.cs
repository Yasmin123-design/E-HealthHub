namespace E_PharmaHub.Dtos
{
    public class WeeklyCategoryItemsDto
    {
        public List<CategoryItemsCountDto> ThisWeek { get; set; }
        public List<CategoryItemsCountDto> LastWeek { get; set; }
    }
}
