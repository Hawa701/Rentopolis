using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Data;

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
        public async Task<IActionResult> Home()
        {
            var managerList = await _services.GetUsersByRole("Manager",null);
            var landlordList = await _services.GetUsersByRole("Landlord",null);
            var tenantList = await _services.GetUsersByRole("Tenant",null);

            ViewBag.ManagerCount = managerList.Count;
            ViewBag.LandlordCount = landlordList.Count;
            ViewBag.TenantCount = tenantList.Count;
            ViewBag.PropertyCount = await _services.NumberOfAllProperty();
            ViewBag.SavedCount = await _services.NumberOfSavedProperty();
            ViewBag.RequestedCount = await _services.NumberOfRequestedProperty();
            ViewBag.ReportCount = await _services.NumberOfReports();

            return View();
        }


        // Users List
        [HttpGet]
        public async Task<IActionResult> AllUsers(string role, string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            var users = await _services.GetUsersByRole(role, SearchString);
            TempData["role"] = role;
            return View(users);
        }


        // Add managers
        [HttpGet]
        public IActionResult AddManager()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddManager(RegisterationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.CreateNewManager(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = returnedStatus.StatusMessage;
                return RedirectToAction("AllUsers", "Admin", new { role = "Manager" });
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // Delete managers
        [HttpGet]
        public async Task<IActionResult> DeleteManager(string id)
        {
            Status returnedStatus = await _services.DeleteManager(id);

            if (returnedStatus.StatusCode == 0) TempData["deletionErrors"] = returnedStatus.StatusMessage;
            if (returnedStatus.StatusCode == 1) TempData["successMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("AllUsers", "Admin", new { role = "Manager" });
        }
    }
}
