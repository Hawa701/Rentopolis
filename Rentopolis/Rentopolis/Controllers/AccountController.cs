using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserAuthenticationServices _services;
        public AccountController(IUserAuthenticationServices userAuthenticationServices)
        {
            this._services = userAuthenticationServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Status returnedStatus = await _services.LoginAsync(model);
            if (returnedStatus.StatusCode == 1)
            {
                string role = await _services.GetUserRoleAsync(model.UserName);

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _services.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Registeration model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Status returnedStatus = await _services.RegisterAsync(model);
            TempData["msg"] = returnedStatus.StatusMessage;
            return RedirectToAction("Login");
        }

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
