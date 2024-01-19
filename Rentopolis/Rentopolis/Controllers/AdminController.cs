using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminServices _services;
        public AdminController(IAdminServices adminServices)
        {
            this._services = adminServices;
        }

        // Home page
        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }


        // Users List
        [HttpGet]
        public async Task<IActionResult> ListUsers(string role)
        {
            var users = await _services.GetUsersByRole(role);

            return View(users);
        }
    }
}
