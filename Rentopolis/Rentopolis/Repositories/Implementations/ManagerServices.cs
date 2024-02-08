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


        // Get number of non-approved properties
        public async Task<int> GetNumberOfNonApprovedProperties()
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.IsApproved == false && p.IsDeleted == false).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties.Count;
        }


        // Get number of total reports
        public async Task<int> NumberOfReports()
        {
            List<ReportedUsers> reportList = new List<ReportedUsers>();

            try
            {
                reportList = await rentContext.ReportedUsers.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return reportList.Count;

        }


        // Get number of banned users
        public async Task<int> GetNumberOfBannedUsers()
        {
            var reportedUserIds = await rentContext.ReportedUsers.Select(r => r.UserId).ToListAsync();

            var userList = await userManager.Users.Where(u => reportedUserIds.Contains(u.Id)).ToListAsync();

            var bannedUsers = userList.Where(u => u.LockoutEnd != null).ToList();

            return bannedUsers.Count;
        }


        // Get number of unbanned users
        public async Task<int> GetNumberOfUnbannedUsers()
        {
            var reportedUserIds = await rentContext.ReportedUsers.Select(r => r.UserId).ToListAsync();

            var userList = await userManager.Users.Where(u => reportedUserIds.Contains(u.Id)).ToListAsync();

            var unbannedUsers = userList.Where(u => u.LockoutEnd == null).ToList();

            return unbannedUsers.Count();
        }
    }
}
