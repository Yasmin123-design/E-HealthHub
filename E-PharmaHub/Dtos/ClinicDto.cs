namespace E_PharmaHub.Dtos
{
    public class ClinicDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? ImagePath { get; set; }
        public int AddressId { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
