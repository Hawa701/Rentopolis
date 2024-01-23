using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class NewPropertyViewModel
    {
        //public string ImageUrl { get; set; }
        [Required]
        public string LandlordId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        public string Features { get; set; }

        [Required]
        public int Bedroom { get; set; }

        [Required]
        public int Bathroom { get; set; }

        [Required]
        public decimal Area { get; set; }

        [Required]
        public decimal PricePerMonth { get; set; }
    }
}
