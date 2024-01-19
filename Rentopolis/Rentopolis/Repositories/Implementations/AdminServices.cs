using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class AdminServices : IAdminServices
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IPasswordHasher<AppUser> passwordHasher;

        public AdminServices(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IPasswordHasher<AppUser> passwordHasher)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.passwordHasher = passwordHasher;
        }

        public async Task<List<AppUser>> GetUsersByRole(string role)
        {
            var userList = await userManager.GetUsersInRoleAsync(role);

            return userList.ToList();
        }
    }
}
