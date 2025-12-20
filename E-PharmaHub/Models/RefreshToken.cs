namespace E_PharmaHub.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string TokenHash { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }

}
