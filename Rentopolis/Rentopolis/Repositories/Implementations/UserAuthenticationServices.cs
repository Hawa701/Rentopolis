using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Security.Claims;

namespace Rentopolis.Repositories.Implementations
{
    public class UserAuthenticationServices : IUserAuthenticationServices
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAuthenticationServices(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // For Login
        public async Task<Status> LoginAsync(Login model)
        {
            Status status = new Status();
            var user = await userManager.FindByNameAsync(model.UserName);
            // if user with the given username doesn't exist
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that user name doesn't exist!";
                return status;
            }

            // if user with the given password doesn't exist
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid password!";
                return status;
            }

            // if username and password match
            SignInResult signInResult = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                };

                foreach (var userRole in userRoles)
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                status.StatusCode = 1;
                status.StatusMessage = "Logged in successfully!";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid user name!";
                return status;
            }
        }

        // For Logout
        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        // For Registering
        public async Task<Status> RegisterAsync(Registeration model)
        {
            Status status = new Status();
            var resut = await userManager.FindByNameAsync(model.UserName);
            //if username exists
            if (resut != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that username already exists! Please try another one.";
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
            };

            IdentityResult result = await userManager.CreateAsync(user, model.Password);
            //if it failed
            if (!result.Succeeded)
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
            status.StatusMessage = "User is registered successfully!";
            return status;
        }

        // For Getting Role of a user
        public async Task<string> GetUserRoleAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);

                if (roles.Any()) return roles.FirstOrDefault();
            }

            return null; // or any default role if needed
        }
    }
}
