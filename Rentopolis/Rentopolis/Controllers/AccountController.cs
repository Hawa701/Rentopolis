using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Security.Claims;

namespace Rentopolis.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountServices _services;
        public AccountController(IAccountServices accountServices)
        {
            this._services = accountServices;
        }


        // Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.LoginUser(model);
            if (returnedStatus.StatusCode == 1)
            {
                ViewBag.Success = true;

                string role = User.FindFirstValue(ClaimTypes.Role);

                if (role == "Admin") return RedirectToAction("Home", "Admin");
                else if (role == "Manager") return RedirectToAction("Home", "Manager");
                else if (role == "Landlord") return RedirectToAction("Home", "Landlord");
                else if (role == "Tenant") return RedirectToAction("Home", "Tenant");
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return View(model);
            }

        }


        // Logout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _services.LogoutUser();
            return RedirectToAction(nameof(Login));
        }


        // Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.RegisterUser(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = "Success";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // Own Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyProfile(string id)
        {
            FullInfoViewModel user = await _services.GetUserByIdForView(id);

            List<ReportedUsers> reportsOnUser = await _services.GetReportsByUserId(user.Id);
            ViewBag.Reports = reportsOnUser;

            return View(user);
        }


        // Other user profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserProfile(string id)
        {
            FullInfoViewModel user = await _services.GetUserByIdForView(id);

            List<ReportedUsers> reportsOnUser = await _services.GetReportsByUserId(user.Id);
            ViewBag.Reports = reportsOnUser;

            return View(user);
        }


        // Edit Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string id)
        {
            UpdateUserInfoViewModel user = await _services.GetUserByIdForEdit(id);
            
            if (user == null) 
            {
                TempData["errorMessage"] = "Couldn't find the username!";
                return View(user);
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(UpdateUserInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.EditUserProfile(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = returnedStatus.StatusMessage;
                return RedirectToAction("MyProfile", "Account", new { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }
            else
            {
                ViewBag.Success = false;
                TempData["editError"] = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // Change Password
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string id)
        {
            PasswordViewModel model = await _services.GetUserByIdForPassword(id);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.ChangeUserPassword(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = returnedStatus.StatusMessage;
                return RedirectToAction("MyProfile", new { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }
            else
            {
                ViewBag.Success = false;
                TempData["changeError"] = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // Delete Profile
        [HttpGet]
        [Authorize(Roles = "Landlord,Tenant")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            Status returnedStatus = await _services.DeleteUserProfile(id);
            if (returnedStatus.StatusCode == 0)
            {
                TempData["deletionError"] = returnedStatus.StatusMessage;
                return RedirectToAction("MyProfile", "Account", new { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            } 
            return await Logout();
        }


        // When unauthorized
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // Reporting a user
        [HttpPost]
        [Authorize(Roles = "Landlord,Tenant")]
        public async Task<IActionResult> ReportUser(string userId, string message, int propertyId)
        {
            Status returnedStatus = await _services.ReportUser(userId, message);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            if(propertyId == -1)
                return RedirectToAction("UserProfile", "Account", new { id = userId });
            else
                return RedirectToAction("Applicants", "Property", new { propertyId = propertyId });
                
        }


        // For registering the admin

        //public async Task<IActionResult> regAdmin()
        //{
        //    Registeration model= new Registeration
        //    {
        //        UserName = "admin",
        //        Email = "admin@gmail.com",
        //        Password = "Admin@123",
        //        Role = "Admin",
        //    };
        //    Status returnedStatus = await _services.RegisterAsync(model);
        //    return Ok(returnedStatus);
        //}

    }
}
