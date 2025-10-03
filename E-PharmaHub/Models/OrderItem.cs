using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class OrderItem
    {
        [Key] public int Id { get; set; }
        public int OrderId { get; set; }
        public int MedicationId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public virtual Order Order { get; set; }
        public virtual Medication Medication { get; set; }
    }
}
