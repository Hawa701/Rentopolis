using Microsoft.AspNetCore.Identity;

namespace Rentopolis.Models.Data
{
    public class AppUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
    }
}
