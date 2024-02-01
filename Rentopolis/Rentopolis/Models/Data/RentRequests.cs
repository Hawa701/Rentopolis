namespace Rentopolis.Models.Data
{
    public class RentRequests
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public string TenantId { get; set; }
        public AppUser Tenant { get; set; }
        public string Status { get; set; }
    }
}
