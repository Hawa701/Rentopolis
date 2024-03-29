﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rentopolis.Models.Data;
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
                TempData["successMessage"] = returnedStatus.StatusMessage;
                return RedirectToAction("MyProperties", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
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
            ViewBag.PropertyCount = await _services.GetNumberOfPropertiesOwned(id);
            ViewBag.ApprovedCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Approved");
            ViewBag.RejectedCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Rejected");
            ViewBag.WaitingCount = await _services.GetNumberOfOwnedPropertyStatus(id, "Waiting");
            ViewBag.SignedInUsername = @User.FindFirstValue(ClaimTypes.Name);

            return View(propertyList);
        }


        // For listing approved properties (for tenants)
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Listings(string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> propertyList = await _services.GetApprovedProperties(SearchString);

            ViewBag.SignedInUsername = @User.FindFirstValue(ClaimTypes.Name);
            ViewBag.ApprovedCount = await _services.GetApprovedPropertiesNumber();

            return View(propertyList);
        }


        // For viewing property detail
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Property propDetail = await _services.GetPropertyDetail(id);

            string tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);   //getting signed in tenant id

            var savedResult = await _services.IsAreadySaved(id,tenantId);       //checking if property is already saved or not
            var requestResult = await _services.IsAreadyRequested(id,tenantId); //checking if property is already requested or not
            var acceptedResult = await _services.IsAccepted(id,tenantId);       //checking if the property is accepted or not
            var rejectedResult = await _services.IsRejected(id,tenantId);       //checking if the property is rejected or not
            
            List<AppUser> applicantList = await _services.GetApplicants(id);    //getting all the applicants that applied
            ViewBag.ApplicantCount = applicantList.Count;

            if (savedResult != null) ViewBag.IsSaved = true;
            else ViewBag.IsSaved = false;

            if (requestResult != null) ViewBag.IsRequested = true;
            else ViewBag.IsRequested = false;

            if (acceptedResult != null) ViewBag.IsAccepted = true;
            else ViewBag.IsAccepted = false;

            if (rejectedResult != null) ViewBag.IsRejeceted = true;
            else ViewBag.IsRejeceted = false;


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
                TempData["successMessage"] = returnedStatus.StatusMessage;
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
                TempData["successMessage"] = returnedStatus.StatusMessage;
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


        // For resending a property for a manager again (for approval)
        [HttpPost]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> ReapplyProperty(int propertyId)
        {
            Status returnedStatus = await _services.ReapplyRejectedProperty(propertyId);

            if (returnedStatus.StatusCode == 0) TempData["failureMessage"] = returnedStatus.StatusMessage;
            if (returnedStatus.StatusCode == 1) TempData["successMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("Detail", "Property", new { id = propertyId }); ;
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
                TempData["deletionError"] = returnedStatus.StatusMessage;

                return RedirectToAction("Detail", "Property", new { id = id });
            }
            else
            {
                TempData["successMessage"] = returnedStatus.StatusMessage;

                if(role == "Landlord") 
                    return RedirectToAction("MyProperties", "Property", new { id = User.FindFirstValue(ClaimTypes.NameIdentifier) });
                else 
                    return RedirectToAction("AllProperties", "Property");
            }
        }


        // For saving a property
        [HttpPost]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> SaveUnsaveProperty(int propertyId, string view)
        {
            string tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Status returnedStatus = await _services.HandleSaveRequest(propertyId, tenantId);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction(view, "Property", new { id = propertyId });
        }


        // For displaying saved propeties
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Favorites(string id, string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> savedList = await _services.GetSavedProperties(id, SearchString);
            return View(savedList);
        }


        // For requesting a rent
        [HttpPost]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> RequestRent(int propertyId)
        {
            string tenantId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Status returnedStatus = await _services.HandleRentalRequest(propertyId, tenantId);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("Detail", "Property", new { id = propertyId });
        }


        // For displaying requested propeties
        [HttpGet]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> Requests(string id, string SearchString = null)
        {
            ViewData["CurrentFilter"] = SearchString;
            List<Property> requestedList = await _services.GetRequestedProperties(id, SearchString);
            return View(requestedList);
        }


        // For displaying property requestors
        [HttpGet]
        [Authorize(Roles = "Landlord, Admin")]
        public async Task<IActionResult> Applicants(int propertyId)
        {
            List<AppUser> applicantList = await _services.GetApplicants(propertyId);

            Dictionary<string, string> userIds = new Dictionary<string, string>();
            Dictionary<string, string> userStatuses = new Dictionary<string, string>();

            foreach (var applicant in applicantList)
            {
                userIds[applicant.Id] = applicant.Id;
                var acceptedResult = await _services.IsAccepted(propertyId, applicant.Id);
                var rejectedResult = await _services.IsRejected(propertyId, applicant.Id);

                if (acceptedResult != null && rejectedResult == null)
                    userStatuses[applicant.Id] = "Accepted";
                else if (rejectedResult != null && acceptedResult == null)
                    userStatuses[applicant.Id] = "Rejected";
                else
                    userStatuses[applicant.Id] = "Waiting";
            }

            ViewBag.UserIds = userIds;
            ViewBag.UserStatuses = userStatuses;

            ViewBag.PropertyId = propertyId;
            return View(applicantList);
        }


        // For accepting tenants request
        [HttpGet]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> AcceptRequest(int propertyId, string tenantId)
        {
            Status returnedStatus = await _services.AcceptApplicantsRequest(propertyId, tenantId);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("Applicants", "Property", new { propertyId = propertyId });
        }


        // For accepting tenants request
        [HttpGet]
        [Authorize(Roles = "Landlord")]
        public async Task<IActionResult> RevertRequest(int propertyId, string tenantId)
        {
            Status returnedStatus = await _services.UndoApplicantsRequest(propertyId, tenantId);

            if (returnedStatus.StatusCode == 1)
                TempData["successMessage"] = returnedStatus.StatusMessage;
            else
                TempData["failureMessage"] = returnedStatus.StatusMessage;

            return RedirectToAction("Applicants", "Property", new { propertyId = propertyId });
        }
        

    }
}
