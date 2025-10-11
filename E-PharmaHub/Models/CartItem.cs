using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public virtual Cart? Cart { get; set; }
        public virtual Medication? Medication { get; set; }
    }
}
