namespace Rentopolis.Models.Data
{
    public class SavedProperties
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }
        public string TenantId { get; set; }
        public AppUser Tenant { get; set; }
    }
}
