using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class PaymentRequestDto
    {
        public int? DoctorId { get; set; }
        public int? PharmacistId { get; set; }
        public string? ReferenceId { get; set; }
        public PaymentForType PaymentFor { get; set; }
        public decimal? Amount { get; set; }
        public int? OrderId { get; set; }
        public int? AppointmentId { get; set; }


    }
}
