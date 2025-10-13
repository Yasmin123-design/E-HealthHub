using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pharmacy name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Pharmacy name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public int AddressId { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Phone number can't exceed 15 digits.")]
        public string Phone { get; set; }

        [StringLength(255, ErrorMessage = "Image path too long.")]
        public string? ImagePath { get; set; }

        public virtual Address? Address { get; set; }
        public virtual ICollection<InventoryItem>? Inventory { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
    }

    
}
