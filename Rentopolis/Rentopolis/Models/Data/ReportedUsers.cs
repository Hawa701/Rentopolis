namespace Rentopolis.Models.Data
{
    public class ReportedUsers
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string Message { get; set; }
    }
}
