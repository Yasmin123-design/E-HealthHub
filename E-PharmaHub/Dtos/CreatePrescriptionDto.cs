namespace E_PharmaHub.Dtos
{
    public class CreatePrescriptionDto
    {
        public string UserId { get; set; }
        public int? DoctorId { get; set; }
        public List<PrescriptionItemDto> Items { get; set; }
    }
}
