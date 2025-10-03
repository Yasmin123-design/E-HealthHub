using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Pharmacy
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public int AddressId { get; set; }
        public string Phone { get; set; }
        public double Rating { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<InventoryItem> Inventory { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }

}
