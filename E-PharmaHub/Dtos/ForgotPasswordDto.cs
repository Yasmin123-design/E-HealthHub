using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
