using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Data;

namespace Rentopolis.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly IManagerServices _services;
        public ManagerController(IManagerServices managerServices)
        {
            this._services = managerServices;
        }


        // Home page
        public async Task<IActionResult> Home()
        {

            var reportedLandlordList = await _services.GetReportedUsersByRole("Landlord",null);
            var reportedTenantList = await _services.GetReportedUsersByRole("Tenant",null);

            ViewBag.NonApprovedPropertyCount = await _services.GetNumberOfNonApprovedProperties();
            ViewBag.LandlordCount = reportedLandlordList.Count;
            ViewBag.TenantCount = reportedTenantList.Count;
            ViewBag.ReportCount = await _services.NumberOfReports();
            ViewBag.BannedCount = await _services.GetNumberOfBannedUsers();
            ViewBag.UnbannedCount = await _services.GetNumberOfUnbannedUsers();

            return View();
        }


        // Users list
        [HttpGet]
        public async Task<IActionResult> ReportedUsers(string role, string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            var users = await _services.GetReportedUsersByRole(role, SearchString);
            
            TempData["role"] = role;
            return View(users);
        }


        //Ban users
        [HttpGet]
        public async Task<IActionResult> BanUser(string id, string role)
        {
            Status returnedStatus = await _services.BanUser(id);

            if (returnedStatus.StatusCode == 0) TempData["deletionErrors"] = returnedStatus.StatusMessage;
            if (returnedStatus.StatusCode == 1) TempData["successMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("ReportedUsers", "Manager", new { role = role });
        }
        

        //Unban users
        [HttpGet]
        public async Task<IActionResult> UnbanUser(string id, string role)
        {
            Status returnedStatus = await _services.UnbanUser(id);

            if (returnedStatus.StatusCode == 0) TempData["deletionErrors"] = returnedStatus.StatusMessage;
            if (returnedStatus.StatusCode == 1) TempData["successMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("ReportedUsers", "Manager", new { role = role });
        }
    }
}
