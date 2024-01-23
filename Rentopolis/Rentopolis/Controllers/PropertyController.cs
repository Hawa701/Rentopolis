using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using Property = Rentopolis.Models.Data.Property;

namespace Rentopolis.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyServices _services;
        public PropertyController(IPropertyServices propertyServices)
        {
            this._services = propertyServices;
        }

        // For creating a property
        [HttpGet]
        [Authorize(Roles = "Landlord")]
        public IActionResult CreateProperty(string id)
        {
            ViewBag.LandlordId = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> CreateProperty(NewPropertyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.AddNewProperty(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = "Success";
                return RedirectToAction("Home", "Landlord");
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // For listing all properties (for admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllProperties()
        {
            List<Property> propertyList = await _services.GetAllProperties();
            return View(propertyList);
        }


        // For listing unapproved properties (for manager)
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> PendingProperties()
        {
            List<Property> propertyList = await _services.GetNonApprovedProperties();
            return View(propertyList);
        }


        // For listing own properties (for landlords)
        [HttpGet]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> MyProperties(string id)
        {
            List<Property> propertyList = await _services.GetOwnedProperties(id);
            ViewBag.LandlordId = id;
            return View(propertyList);
        }


        // For listing approved properties (for tenants)
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Listings()
        {
            List<Property> propertyList = await _services.GetApprovedProperties();
            return View(propertyList);
        }
    }
}
