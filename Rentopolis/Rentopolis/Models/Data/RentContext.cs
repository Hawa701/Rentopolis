using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rentopolis.Models.Data
{
    public class RentContext : IdentityDbContext<AppUser>
    {
        public RentContext(DbContextOptions<RentContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        //public DbSet<PropertyGallery> PropertyGallery { get; set; }
    }
}
