namespace E_PharmaHub.Dtos
{
    public class UserUpdateDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword
        {
            get; set;
        }
    }
}
