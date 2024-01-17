using Microsoft.AspNetCore.Identity;

namespace Rentopolis.Models.Data
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
