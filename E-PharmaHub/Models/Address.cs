using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{


    public class Address
    {
        [Key] public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

}
