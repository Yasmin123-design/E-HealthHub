using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class AlternativeMedication
    {
        [Key] public int Id { get; set; }
        public int MedicationId { get; set; }
        public int AlternativeId { get; set; } 
        public string Reason { get; set; } 
        public virtual Medication Medication { get; set; }
        [ForeignKey("AlternativeId")] public virtual Medication Alternative { get; set; }
    }
}
