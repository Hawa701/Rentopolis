using System.ComponentModel.DataAnnotations.Schema;

namespace Rentopolis.Models.Data
{
    public class Property
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? Features { get; set; }
        public int Bedroom { get; set; }
        public int Bathroom { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Area { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerMonth { get; set; }
        public string LandlordId { get; set; }
        public string? TenantId { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRented { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? RentedDate { get; set; }
        public ICollection<PropertyGallery> propertyGallery { get; set; }
    }
}
