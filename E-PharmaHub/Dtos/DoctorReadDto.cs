using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class DoctorReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Specialty { get; set; }
        public bool IsApproved { get; set; }
        public Gender Gender { get; set; }
        public double AverageRating { get; set; }
        public string Username { get; set; }
        public decimal ConsultationPrice { get; set; }
        public ConsultationType ConsultationType { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public string ClinicPhone { get; set; }
        public string? ClinicImagePath { get; set; }
        public string? DoctorImage { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int CountPatient { get; set; }
        public int CountReviews { get; set; }
        public int CountFavourite { get; set; }
    }

}
