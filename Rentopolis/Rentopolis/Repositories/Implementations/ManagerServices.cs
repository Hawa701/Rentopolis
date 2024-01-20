using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class ManagerServices : IManagerServices
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ManagerServices(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // Get user list
        public async Task<List<AppUser>> GetUsersList(string role)
        {
            if (role == "Landlord" || role == "Tenant")
            {
                var userList = await userManager.GetUsersInRoleAsync(role);
                return userList.ToList();
            }

            return null;
        }

        // Ban user
        public async Task<Status> BanUser (string id)
        {
            Status status = new Status();
            var user = await userManager.FindByIdAsync(id);

            // if no user was found
            if(user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with this Id doesn't exist!";
                return status;
            }

            // if user is locked out
            if (await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "User is already banned!";
                return status;
            }

            IdentityResult result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if(result.Succeeded)
                status.StatusCode = 1;
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to lockout the user!";
            }

            return status;
        }

        // Unban user
        public async Task<Status> UnbanUser (string id)
        {
            Status status = new Status();
            var user = await userManager.FindByIdAsync(id);

            // if no user was found
            if(user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with this Id doesn't exist!";
                return status;
            }

            // if user is locked out
            if (!await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "User is not banned!";
                return status;
            }

            IdentityResult result = await userManager.SetLockoutEndDateAsync(user, null);
            if(result.Succeeded)
                status.StatusCode = 1;
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to unban the user!";
            }

            return status;
        }

    }
}
