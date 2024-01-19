using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Security.Claims;

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
            TempData["role"] = role;
            return View(users);
        }

        // Add managers
        [HttpGet]
        public async Task<IActionResult> NewManager()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewManager(RegisterationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Status returnedStatus = await _services.CreateNewManager(model);
            if (returnedStatus.StatusCode == 0)
            {
                TempData["registrationMsg"] = returnedStatus.StatusMessage;
                return View();
            }
            return RedirectToAction("ListUsers", "Admin", new { role = "Manager" });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteManager(string id)
        {
            Status returnedStatus = await _services.DeleteManager(id);
            if (returnedStatus.StatusCode == 0) TempData["deletionErrors"] = returnedStatus.StatusMessage;
            return RedirectToAction("ListUsers", "Admin", new { role = "Manager" });
        }
    }
}
