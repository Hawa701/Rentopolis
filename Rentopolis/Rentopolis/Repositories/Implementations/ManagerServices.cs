using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class ManagerServices : IManagerServices
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RentContext rentContext;

        public ManagerServices(UserManager<AppUser> userManager, RentContext rentContext)
        {
            this.userManager = userManager;
            this.rentContext = rentContext;
        }

        // Get user list by their role
        public async Task<List<AppUser>> GetReportedUsersByRole(string role, string searchString)
        {
            var userList = await userManager.GetUsersInRoleAsync(role);

            // Filtering out reported users id
            var reportedUserIds = await rentContext.ReportedUsers.Select(r => r.UserId).ToListAsync();
            userList = userList.Where(u => reportedUserIds.Contains(u.Id)).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                userList = userList.Where(u =>
                    u.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.UserName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.Email.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }

            return userList.ToList();
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
            if (result.Succeeded) { 
                status.StatusCode = 1;
                status.StatusMessage = "User banned successfully!";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to ban the user!";
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
            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.StatusMessage = "User unbanned successfully!";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to unban the user!";
            }

            return status;
        }

    }
}
