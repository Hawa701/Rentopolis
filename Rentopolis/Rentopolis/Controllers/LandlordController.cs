using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Repositories.Interfaces;
using System.Security.Claims;

namespace Rentopolis.Controllers
{
    public class LandlordController : Controller
    {

        private readonly ILandlordServices _services;
        public LandlordController(ILandlordServices landlordServices)
        {
            this._services = landlordServices;
        }


        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> Home()
        {
            TempData["userId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string id = TempData["userId"].ToString();

            ViewBag.ApprovedCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Approved");
            ViewBag.RejectedCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Rejected");
            ViewBag.WaitingCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Waiting");

            return View();
        }
    }
}
