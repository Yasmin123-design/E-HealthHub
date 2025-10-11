using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class CheckoutDto
    {
        public int PharmacyId { get; set; }

        [Required] public string Country { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Street { get; set; }
        [Required] public string PhoneNumber { get; set; }
    }

}
