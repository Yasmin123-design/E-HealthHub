namespace E_PharmaHub.Dtos
{
    public class PharmacySimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double AverageRating { get; set; }
        public string City { get; set; }
        public string ImagePath { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public double? Latitude { get; set;}
        public double? Longitude { get; set; }

        public double? DistanceFromUser { get; set; }
        public decimal? DeliveryFee { get; set; }


    }
}
