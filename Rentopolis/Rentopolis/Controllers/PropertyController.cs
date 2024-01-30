using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Security.Claims;
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
            TempData["LandlordId"] = id;
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
        public async Task<IActionResult> AllProperties(string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> propertyList = await _services.GetAllProperties(SearchString);
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
        public async Task<IActionResult> MyProperties(string id, string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> propertyList = await _services.GetOwnedProperties(id, SearchString);
            ViewBag.LandlordId = id;
            return View(propertyList);
        }


        // For listing approved properties (for tenants)
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Listings(string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> propertyList = await _services.GetApprovedProperties(SearchString);
            return View(propertyList);
        }


        // For viewing property detail
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Property propDetail = await _services.GetPropertyDetail(id);

            string tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _services.IsAreadySaved(id,tenantId);

            if (result != null)
                ViewBag.IsSaved = true;
            else
                ViewBag.IsSaved = false;

            return View(propDetail);
        }


        // For approving properties (managers work)
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            Status returnedStatus = await _services.ApproveProperty(id);
            if(returnedStatus.StatusCode == 0)
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return RedirectToAction("Detail", new {id = id});
            }
            else
            {
                TempData["successMessage"] = "Success";
                return RedirectToAction("PendingProperties", "Property");
            }
        }


        // For rejecting properties (managers work)
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Reject(int id)
        {
            Status returnedStatus = await _services.RejectProperty(id);
            if (returnedStatus.StatusCode == 0)
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return RedirectToAction("Detail", new { id = id });
            }
            else
            {
                TempData["successMessage"] = "Success";
                return RedirectToAction("PendingProperties", "Property");
            }
        }


        // For editing the property (landlords work)
        [HttpGet]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> EditProperty(int id)
        {
            UpdatePropertyInfoViewModel model = await _services.GetPropertyDetailForEditing(id);

            if (model == null)
            {
                ViewBag.Success = false;
                ViewBag.Message = "A property with that Id doesn't exist!";
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> EditProperty(UpdatePropertyInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Success = false;
                return View(model);
            }

            Status returnedStatus = await _services.EditPropertyInfo(model);

            if (returnedStatus.StatusCode == 1)
            {
                TempData["successMessage"] = "Updated Succesfully!";
                return RedirectToAction("Detail", "Property", new { id = model.Id });
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Message = returnedStatus.StatusMessage;
                return View(model);
            }
        }


        // For deleting the property
        [HttpPost]
        [Authorize(Roles = "Landlord, Admin")]
        public async Task<IActionResult> DeleteProperty(int id, string role)
        {
            Status returnedStatus = await _services.DeletePropertyInfo(id);

            if (returnedStatus.StatusCode == 0)
            {
                ViewBag.Success = 0;
                ViewBag.Message = returnedStatus.StatusMessage;
                return RedirectToAction("Detail", "Property", new { id = id });
            }
            else
            {
                TempData["successMessage"] = "Deleted Succesfully!";

                if(role == "Landlord") return RedirectToAction("MyProperties", "Property", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
                else return RedirectToAction("AllProperties", "Property");
            }
        }


        // For saving a property
        [HttpPost]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> SaveUnsaveProperty(int propertyId, string view)
        {
            string tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Status returnedStatus = await _services.SaveOrUnsaveToSavedProperties(propertyId, tenantId);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction(view, "Property", new { id = propertyId });
        }


        // For displaying saved propeties
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Favorites(string id)
        {
            List<Property> savedList = await _services.GetSavedProperties(id);
            return View(savedList);
        }
    }
}
