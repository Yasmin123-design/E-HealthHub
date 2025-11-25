namespace E_PharmaHub.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public UserProfileDto? User { get; set; }
    }

}
