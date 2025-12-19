namespace E_PharmaHub.Dtos
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public DateTime IssuedAt { get; set; }
        public List<PrescriptionItemDto> Items { get; set; }
    }

}
