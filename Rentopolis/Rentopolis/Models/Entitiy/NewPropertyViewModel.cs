using System.ComponentModel.DataAnnotations;

namespace Rentopolis.Models.Entitiy
{
    public class NewPropertyViewModel
    {
        [Required]
        public string LandlordId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Sub City")]
        public string City { get; set; }

        public string Features { get; set; }

        [Required]
        public int Bedroom { get; set; }

        [Required]
        public int Bathroom { get; set; }

        [Required]
        public decimal Area { get; set; }

        [Required]
        [Display(Name = "Price Per Month")]
        public decimal PricePerMonth { get; set; }

        [Display(Name = "Cover Photo")]
        public IFormFile MainPhoto { get; set; }
        public string? MainPhotoUrl { get; set; } = "";
        public IFormFileCollection GalleryPhotos { get; set; }
        public List<PropertyGalleryModel> Gallery { get; set; }
    }
}
