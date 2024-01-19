using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                return View(model);

            Status returnedStatus = await _services.LoginAsync(model);
            if (returnedStatus.StatusCode == 1)
            {
                //string role = await _services.GetUserRoleAsync(model.UserName);
                string role = User.FindFirstValue(ClaimTypes.Role);

                if (role == "Admin") return RedirectToAction("Home", "Admin");
                else if (role == "Manager") return RedirectToAction("Home", "Manager");
                else if (role == "Landlord") return RedirectToAction("Home", "Landlord");
                else if (role == "Tenant") return RedirectToAction("Home", "Tenant");
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["msg"] = returnedStatus.StatusMessage;
                return RedirectToAction(nameof(Login));
            }

        }


        // Logout
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _services.LogoutAsync();
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
                return View(model);

            Status returnedStatus = await _services.RegisterAsync(model);
            TempData["registrationMsg"] = returnedStatus.StatusMessage;
            return RedirectToAction("Login");
        }


        // View own Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewProfile(string id)
        {
            FullInfoViewModel user = await _services.GetUserById(id);
            return View(user);
        }


        //View other users profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserProfile(string id)
        {
            FullInfoViewModel user = await _services.GetUserById(id);
            return View(user);
        }



        // Edit Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(string id)
        {
            FullInfoViewModel user = await _services.GetUserById(id);
            //user.Role = await _services.GetUserRoleAsync(user.UserName);
            if (user == null)
            {
                TempData["errorMessage"] = "Couldn't find the username!";
                return View(user);
            }

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(FullInfoViewModel model)
        {
            if(!ModelState.IsValid) 
                return View(model);

            Status returnedStatus = await _services.EditUserProfile(model);
            TempData["errorMessage"] = returnedStatus.StatusMessage;
            return RedirectToAction("ViewProfile", "Account", new { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }


        // Delete Profile
        [HttpGet]
        [Authorize(Roles = "Landlord,Tenant")]
        public async Task<IActionResult> DeleteProfile(string id)
        {
            Status returnedStatus = await _services.DeleteUserProfile(id);
            if (returnedStatus.StatusCode == 0)
            {
                TempData["errorMessage"] = returnedStatus.StatusMessage;
                return RedirectToAction("EditProfile", "Account", new { Id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            } 
            return await Logout();
        }

        // When unauthorized
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        //For registering the admin

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
