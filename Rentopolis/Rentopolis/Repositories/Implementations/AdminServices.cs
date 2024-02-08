using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class AdminServices : IAdminServices
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly RentContext rentContext;

        public AdminServices(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, RentContext rentContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.rentContext = rentContext;
        }

        // Get users by their role (list)
        public async Task<List<AppUser>> GetUsersByRole(string role, string searchString)
        {
            var userList = await userManager.GetUsersInRoleAsync(role);

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


        // Create a manager
        public async Task<Status> CreateNewManager(RegisterationViewModel model)
        {
            Status status = new Status();

            //if username already exists
            var result = await userManager.FindByNameAsync(model.UserName);
            if (result != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that username already exists. Please try another!";
                return status;
            }

            //if email already exists
            var result2 = await userManager.FindByEmailAsync(model.Email);
            if (result2 != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that email already exists. Please try another!";
                return status;
            }

            AppUser user = new AppUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.UserName,
                ProfilePicture = "/Images/User Profiles/default.jpg"
            };

            IdentityResult createResult = await userManager.CreateAsync(user, model.Password);
            
            //if it failed
            if (!createResult.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to register the account!";
                return status;
            }

            // checking if a role was specified or not
            bool roleExists = await roleManager.RoleExistsAsync(model.Role);
            if (roleExists) await userManager.AddToRoleAsync(user, model.Role);

            if (!roleExists) await roleManager.CreateAsync(new IdentityRole(model.Role));

            status.StatusCode = 1;
            status.StatusMessage = "Manager profile created successfully!";

            return status;
        }


        // Delete manager
        public async Task<Status> DeleteManager(string id)
        {
            Status status = new Status();
            // When user id is not passed correctly
            if (string.IsNullOrEmpty(id))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Id is null!";
                return status;
            }

            AppUser user = await userManager.FindByIdAsync(id);

            // When user is not found
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A manager with this Id doesn't exist!";
                return status;
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.StatusMessage = "Manager profile deleted successfully!";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to delete the manager profile!";
            }
            return status;
        }


        // Get number of all Properties
        public async Task<int> NumberOfAllProperty()
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties.Count;

        }


        // Get number of saved properties
        public async Task<int> NumberOfSavedProperty()
        {
            List<SavedProperties> savedProperties = new List<SavedProperties>();

            try
            {
                savedProperties = await rentContext.SavedProperties.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return savedProperties.Count;

        }


        // Get number of requested properties
        public async Task<int> NumberOfRequestedProperty()
        {
            List<RentRequests> requestedProperties = new List<RentRequests>();

            try
            {
                requestedProperties = await rentContext.RentalRequests.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return requestedProperties.Count;

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
    }
}
