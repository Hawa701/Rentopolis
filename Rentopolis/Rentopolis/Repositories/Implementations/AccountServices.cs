using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.IO;
using System.Security.Claims;

namespace Rentopolis.Repositories.Implementations
{
    public class AccountServices : IAccountServices
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly RentContext rentContext;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AccountServices(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, RentContext rentContext, IWebHostEnvironment webHostEnvironment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.rentContext = rentContext;
            this.webHostEnvironment = webHostEnvironment;
        }


        // Get user by Id for viewing
        public async Task<FullInfoViewModel> GetUserByIdForView(string id)
        {
            var result = await userManager.FindByIdAsync(id);
            if (result == null) return null;

            FullInfoViewModel user = new FullInfoViewModel
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                UserName = result.UserName,
                Email = result.Email,
                ProfilePicUrl = result.ProfilePicture,
            };

            return user;
        }


        // Get user by Id for editing
        public async Task<UpdateUserInfoViewModel> GetUserByIdForEdit(string id)
        {
            var result = await userManager.FindByIdAsync(id);
            if (result == null) return null;

            UpdateUserInfoViewModel user = new UpdateUserInfoViewModel
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                UserName = result.UserName,
                Email = result.Email,
                ProfilePicUrl = result.ProfilePicture,
            };

            return user;
        }


        //Get user by Id for password
        public async Task<PasswordViewModel> GetUserByIdForPassword(string id)
        {
            var result = await userManager.FindByIdAsync(id);
            if (result == null) return null;

            PasswordViewModel userInfo = new PasswordViewModel
            {
                Id = result.Id,
            };

            return userInfo;
        }


        // For Login
        public async Task<Status> LoginUser(LoginViewModel model)
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

            // if user is locked out
            if(await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Your account has been banned!";
                return status;
            }

            // if user with the given password doesn't exist
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Invalid password!";
                return status;
            }

            // if there are no problems
            SignInResult signInResult = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, model.UserName),
                };

                foreach (var userRole in userRoles)
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                status.StatusCode = 1;
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
        public async Task LogoutUser()
        {
            await signInManager.SignOutAsync();
        }


        // For Registering
        public async Task<Status> RegisterUser(RegisterationViewModel model)
        {
            Status status = new Status();

            //if username already exists
            var result = await userManager.FindByNameAsync(model.UserName);
            if (result != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that username already exists! Please try another one.";
                return status;
            }

            //if email already exists
            var result2 = await userManager.FindByEmailAsync(model.Email);
            if (result2 != null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "A user with that email already exists!";
                return status;
            }

            // if profile pic was given
            if (model.ProfilePic != null)
            {
                string folder = "Images/User Profiles/";
                folder += Guid.NewGuid().ToString() + "_" + model.ProfilePic.FileName;

                model.ProfilePicUrl = "/" + folder;

                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);

                // saving image to folder
                await model.ProfilePic.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            }
            else
                model.ProfilePicUrl = "/Images/User Profiles/default.jpg";

            AppUser user = new AppUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                EmailConfirmed = true,
                UserName = model.UserName,
                ProfilePicture = model.ProfilePicUrl,
            };

            IdentityResult createResult = await userManager.CreateAsync(user, model.Password);
            //if it failed
            if (!createResult.Succeeded)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to register the account!";
                return status;
            }

            // checking if the specified role exists or not
            bool roleExists = await roleManager.RoleExistsAsync(model.Role);
            if (roleExists) await userManager.AddToRoleAsync(user, model.Role);
            if (!roleExists) await roleManager.CreateAsync(new IdentityRole(model.Role));

            status.StatusCode = 1;
            return status;
        }
        

        // For Editing profile
        public async Task<Status> EditUserProfile(UpdateUserInfoViewModel model)
        {
            Status status = new Status();
            var user = await userManager.FindByIdAsync(model.Id);
            
            // if user doesn't exist
            if (user == null) { 
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a user with this Id! Please try again!";
                return status;
            }

            // if user is locked out
            if (await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "You can't make changes because you have been banned!";
                return status;
            }

            // are any fields empty?
            if (
                string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email)
            )
            {
                status.StatusCode= 0;
                status.StatusMessage = "All fields are required!";
                return status;
            }

            // if a new profile pic was given
            if (model.ProfilePic != null)
            {
                string folder = "Images/User Profiles/";
                folder += Guid.NewGuid().ToString() + "_" + model.ProfilePic.FileName;

                model.ProfilePicUrl = "/" + folder;

                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);

                // saving image to folder
                await model.ProfilePic.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            }

            // transfer data
            user.Id = model.Id;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.ProfilePicture = model.ProfilePicUrl == null ? user.ProfilePicture: model.ProfilePicUrl;
            //model.ProfilePicUrl == null?

            // update the profile
            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.StatusMessage = "User updated successfully!";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to update the user profile!";
            }

            return status;
        }
        

        // For Changing Password
        public async Task<Status> ChangeUserPassword(PasswordViewModel model)
        {
            Status status = new Status();

            // if user doesn't exist
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a user with this Id! Please try again!";
                return status;
            }

            // if user is locked out
            if (await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "You can't make changes because you have been banned!";
                return status;
            }

            // if old password doesn't match
            if (!await userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                status.StatusCode = 0;
                status.StatusMessage = "Old password is incorrect!";
                return status;
            }

            // Update the password
            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                status.StatusCode = 1;
                status.StatusMessage = "Password changed successfully!";
            }
            else
            {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to change the password!";
            }

            return status;
        }


        // For deleting profile
        public async Task<Status> DeleteUserProfile(string id)
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
                status.StatusMessage = "A user with this Id doesn't exist!";
                return status;
            }

            // if user is locked out
            if (await userManager.IsLockedOutAsync(user))
            {
                status.StatusCode = 0;
                status.StatusMessage = "You can't terminate your account because you have been banned!";
                return status;
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                status.StatusCode = 1;
                status.StatusMessage = "User profile deleted successfully!";
            }
            else {
                status.StatusCode = 0;
                status.StatusMessage = "Failed to delete the user profile!";
            }
            return status;
        }


        // For reporting a user
        public async Task<Status> ReportUser(string id, string message)
        {
            Status status = new Status();

            ReportedUsers reportedUser = new ReportedUsers
            {
                UserId = id,
                Message = message
            };

            try
            {
                rentContext.ReportedUsers.Add(reportedUser);
                await rentContext.SaveChangesAsync();

                status.StatusCode = 1;
                status.StatusMessage = "User reported successfully!";
            }
            catch (Exception ex)
            {
                status.StatusCode=0;
                status.StatusMessage = ex.Message;
            }

            return status;
        }


        // For getting a users report count
        //public async Task<int> GetReportCountByUserId(string userId)
        //{
        //    var reportCount = await rentContext.ReportedUsers
        //        .CountAsync(r => r.UserId == userId);

        //    return reportCount;
        //}


        // For getting reports made on a user
        public async Task<List<ReportedUsers>> GetReportsByUserId(string userId)
        {
            var reports = await rentContext.ReportedUsers
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return reports;
        }

    }
}
