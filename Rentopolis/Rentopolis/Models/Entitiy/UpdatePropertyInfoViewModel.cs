using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentopolis.Models.Entitiy
{
    public class UpdatePropertyInfoViewModel
    {
        //public string? ImageUrl { get; set; }
        [Required]
        public int Id { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Sub City")]
        public string City { get; set; }

        [Required]
        public string? Features { get; set; }

        [Required]
        public int Bedroom { get; set; }

        [Required]
        public int Bathroom { get; set; }


        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Area { get; set; }


        [Required]
        [Display(Name = "Price Per Month")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerMonth { get; set; }
    }
}
