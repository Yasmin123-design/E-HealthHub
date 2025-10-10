namespace E_PharmaHub.Dtos
{
    public class NearestPharmacyDTO
    {
        public string PharmacyName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Phone { get; set; }
        public string MedicationName { get; set; }
        public double DistanceKm { get; set; }
    }
}
