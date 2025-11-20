namespace E_PharmaHub.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string? Image { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string UserEmail { get; set; }
    }

}
