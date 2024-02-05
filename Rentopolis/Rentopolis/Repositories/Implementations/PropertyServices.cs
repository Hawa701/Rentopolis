using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class PropertyServices : IPropertyServices
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly RentContext rentContext;

        public PropertyServices(IWebHostEnvironment webHostEnvironment, RentContext rentContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.rentContext = rentContext;
        }


        // Add new property
        public async Task<Status> AddNewProperty(NewPropertyViewModel model)
        {
            Status status = new Status();

            // if main photo was given
            if (model.MainPhoto != null)
            {
                string folder = "Images/Property Images/Main/";

                model.MainPhotoUrl = await UploadImage(folder, model.MainPhoto);
            }
            else
                model.MainPhotoUrl = "/Images/Property Images/Main/default.jpg";

            // if gallery photos were given
            if (model.GalleryPhotos != null)
            {
                string folder = "Images/Property Images/Gallery/";

                model.Gallery = new List<PropertyGalleryModel>();

                foreach(var file in model.GalleryPhotos)
                {
                    var gallery = new PropertyGalleryModel()
                    {
                        Name = file.Name,
                        URL = await UploadImage(folder, file)
                    };
                    model.Gallery.Add(gallery);
                }
            }

            Property newProperty = new Property
            {
                Address = model.Address,
                City = model.City,
                Features = model.Features,
                Bedroom = model.Bedroom,
                Bathroom = model.Bathroom,
                Area = model.Area,
                PricePerMonth = model.PricePerMonth,
                LandlordId = model.LandlordId,
                IsApproved = false,
                IsDeleted = false,
                IsRented = false,
                AddedDate = DateTime.Now,
                ImageUrl = model.MainPhotoUrl,
            };

            newProperty.propertyGallery = new List<PropertyGallery>();

            foreach(var file in model.Gallery)
            {
                newProperty.propertyGallery.Add(new PropertyGallery()
                {
                    Name = file.Name,
                    URL = file.URL
                });
            }

            try
            {
                await rentContext.Properties.AddAsync(newProperty);
                await rentContext.SaveChangesAsync();

                status.StatusCode = 1;
                status.StatusMessage = "Property created successfully!";
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.StatusMessage = ex.Message;
            }


            return status;
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }


        // Get total owned properties number
        public async Task<int> GetNumberOfPropertiesOwned(string id)
        {
            List<Property> propList = new List<Property>();

            try
            {
                propList = await rentContext.Properties.Where(p => p.LandlordId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return propList.Count;
        }


        // Get number of properties (owned) based on the specified status
        public async Task<int> GetNumberOfOwnedPropertyStatus(string id, string neededStatus)
        {
            List<Property> propList = new List<Property>();

            try
            {
                propList = await rentContext.Properties.Where(p => p.LandlordId == id).ToListAsync();
                if(neededStatus == "Approved")
                {
                    propList = propList.Where(p => p.IsApproved == true && p.IsDeleted == false).ToList();
                }
                else if(neededStatus == "Rejected")
                {
                    propList = propList.Where(p => p.IsApproved == false && p.IsDeleted == true).ToList();
                }
                else if (neededStatus == "Waiting")
                {
                    propList = propList.Where(p => p.IsApproved == false && p.IsDeleted == false).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return propList.Count;
        }


        // List all properties
        public async Task<List<Property>> GetAllProperties(string searchString)
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    properties = properties.Where(p =>
                        p.Address.ToLower().Contains(searchString) ||
                        p.City.ToLower().Contains(searchString) ||
                        p.Bedroom.ToString().Contains(searchString) ||
                        p.Bathroom.ToString().Contains(searchString) ||
                        p.Area.ToString().Contains(searchString) ||
                        p.PricePerMonth.ToString().Contains(searchString)
                    )
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties;

        }


        // List non-approved properties
        public async Task<List<Property>> GetNonApprovedProperties()
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.IsApproved == false && p.IsDeleted == false).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties;
        }


        // List owned properties
        public async Task<List<Property>> GetOwnedProperties(string id, string searchString)
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.LandlordId == id).ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    properties = properties.Where(p =>
                        p.Address.ToLower().Contains(searchString) ||
                        p.City.ToLower().Contains(searchString) ||
                        p.Bedroom.ToString().Contains(searchString) ||
                        p.Bathroom.ToString().Contains(searchString) ||
                        p.Area.ToString().Contains(searchString) ||
                        p.PricePerMonth.ToString().Contains(searchString)
                    )
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties;
        }


        // List approved properties
        public async Task<List<Property>> GetApprovedProperties(string searchString)
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.IsApproved == true).ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    properties = properties.Where(p =>
                        p.Address.ToLower().Contains(searchString) ||
                        p.City.ToLower().Contains(searchString) ||
                        p.Bedroom.ToString().Contains(searchString) ||
                        p.Bathroom.ToString().Contains(searchString) ||
                        p.Area.ToString().Contains(searchString) ||
                        p.PricePerMonth.ToString().Contains(searchString)
                    )
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties;
        }


        // Get approved properties number
        public async Task<int> GetApprovedPropertiesNumber()
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.IsApproved == true).ToListAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties.Count;
        }


        // Get property detail
        public async Task<Property> GetPropertyDetail(int id)
        {
            Property property = new Property();

            try
            {
                property = await rentContext.Properties.Include(p => p.propertyGallery).FirstOrDefaultAsync(p => p.Id == id);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the property detail: " + ex.Message);
            }

            return property;
        }


        // Approve Property
        public async Task<Status> ApproveProperty(int id)
        {
            Status status = new Status();

            Property property = new Property();
            property = await GetPropertyDetail(id);

            // if property is null
            if(property == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a property with that Id!";
            }
            else
            {
                property.IsApproved = true;
                property.ApprovedDate = DateTime.Now;
                property.IsDeleted = false;
                try
                {
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Property is approved successfully!";
                }
                catch (Exception ex)
                {
                    status.StatusCode = 0;
                    status.StatusMessage = ex.Message;
                }
            }

            return status;
        }


        // Reject Property
        public async Task<Status> RejectProperty(int id)
        {
            Status status = new Status();

            Property property = new Property();
            property = await GetPropertyDetail(id);

            // if property is null
            if (property == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a property with that Id!";
            }
            else
            {
                property.IsDeleted = true;
                property.IsApproved = false;
                try
                {
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Property is rejected successfully!";
                }
                catch (Exception ex)
                {
                    status.StatusCode = 0;
                    status.StatusMessage = ex.Message;
                }
            }

            return status;
        }


        // Getting detail for editing
        public async Task<UpdatePropertyInfoViewModel> GetPropertyDetailForEditing(int id)
        {
            Property property = new Property();
            UpdatePropertyInfoViewModel model = new UpdatePropertyInfoViewModel();

            try
            {
                property = await rentContext.Properties.FindAsync(id);

                model.Id = property.Id;
                //model.ImageUrl = property.ImageUrl;
                model.Address = property.Address;
                model.City = property.City;
                model.Features = property.Features;
                model.Bedroom = property.Bedroom;
                model.Bathroom = property.Bathroom;
                model.Area = property.Area;
                model.PricePerMonth = property.PricePerMonth;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the property detail: " + ex.Message);
            }

            return model;
        }


        // Edit Property
        public async Task<Status> EditPropertyInfo(UpdatePropertyInfoViewModel model)
        {
            Status status = new Status();
            Property property = await GetPropertyDetail(model.Id);

            // if property doesn't exist
            if (property == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a property with this Id! Please try again!";
                return status;
            }

            // transfer data
            property.Address = model.Address;
            property.City = model.City;
            property.Features = model.Features;
            property.Bedroom = model.Bedroom;
            property.Bathroom = model.Bathroom;
            property.Area = model.Area;
            property.PricePerMonth = model.PricePerMonth;

            // update the property
            try
            {
                await rentContext.SaveChangesAsync();
                status.StatusCode = 1;
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.StatusMessage = ex.Message;
            }

            return status;
        }


        // Delete Property
        public async Task<Status> DeletePropertyInfo(int id)
        {
            Status status = new Status();
            Property property = await GetPropertyDetail(id);

            // if property doesn't exist
            if (property == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Couldn't find a property with this Id! Please try again!";
                return status;
            }

            try
            {
                rentContext.Properties.Remove(property);
                await rentContext.SaveChangesAsync();
                status.StatusCode = 1;
                status.StatusMessage = "Property deleted successfully";
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.StatusMessage = ex.Message;
            }

            return status;
        }


        // Check if Property is already saved or not
        public async Task<SavedProperties> IsAreadySaved(int propertyId, string tenantId)
        {
            var savedProperty = await rentContext.SavedProperties
                        .FirstOrDefaultAsync(sp => sp.PropertyId == propertyId && sp.TenantId == tenantId);

            return savedProperty;
        }


        // Save or Unsave Property
        public async Task<Status> HandleSaveRequest(int propertyId, string tenantId)
        {
            Status status = new Status();

            try
            {
                var result = await IsAreadySaved(propertyId, tenantId);

                if (result != null)
                {
                    // Property is already saved, so unsave it
                    rentContext.SavedProperties.Remove(result);
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Property removed from Favorites!";
                }
                else
                {
                    // Property is not saved, so save it
                    result = new SavedProperties
                    {
                        PropertyId = propertyId,
                        TenantId = tenantId
                    };

                    rentContext.SavedProperties.Add(result);
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Property saved to Favorites!";
                }
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.StatusMessage = ex.Message;
            }

            return status;
        }


        // Get Saved Properties
        public async Task<List<Property>> GetSavedProperties(string tenantId, string searchString)
        {
            List<Property> savedProperties = new List<Property>();

            try
            {
                var savedPropertyIds = await rentContext.SavedProperties
                    .Where(sp => sp.TenantId == tenantId)
                    .Select(sp => sp.PropertyId)
                    .ToListAsync();

                savedProperties = await rentContext.Properties
                    .Where(p => savedPropertyIds.Contains(p.Id))
                    .ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    savedProperties = savedProperties.Where(p =>
                        p.Address.ToLower().Contains(searchString) ||
                        p.City.ToLower().Contains(searchString) ||
                        p.Bedroom.ToString().Contains(searchString) ||
                        p.Bathroom.ToString().Contains(searchString) ||
                        p.Area.ToString().Contains(searchString) ||
                        p.PricePerMonth.ToString().Contains(searchString)
                    )
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving saved properties: " + ex.Message);
            }

            return savedProperties;
        }


        // Check if Property is already requested or not
        public async Task<RentRequests> IsAreadyRequested(int propertyId, string tenantId)
        {
            var requestedProperties = await rentContext.RentalRequests
                        .FirstOrDefaultAsync(sp => sp.PropertyId == propertyId && sp.TenantId == tenantId);

            return requestedProperties;
        }


        // Send or cancel Rental request
        public async Task<Status> HandleRentalRequest(int propertyId, string tenantId)
        {
            Status status = new Status();

            try
            {
                var result = await IsAreadyRequested(propertyId, tenantId);

                if (result != null)
                {
                    // Rent is already requested, so cancel it
                    rentContext.RentalRequests.Remove(result);
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Rental request canceled!";
                }
                else
                {
                    // Rent is not requested, so make a request
                    result = new RentRequests()
                    {
                        PropertyId = propertyId,
                        TenantId = tenantId,
                        Status = "Waiting"
                    };

                    rentContext.RentalRequests.Add(result);
                    await rentContext.SaveChangesAsync();
                    status.StatusCode = 1;
                    status.StatusMessage = "Rental request sent!";
                }
            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.StatusMessage = ex.Message;
            }

            return status;
        }


        // Get Requested Properties
        public async Task<List<Property>> GetRequestedProperties(string tenantId, string searchString)
        {
            List<Property> requestedProperties = new List<Property>();

            try
            {
                var requestedPropertyIds = await rentContext.RentalRequests
                    .Where(rp => rp.TenantId == tenantId)
                    .Select(rp => rp.PropertyId)
                    .ToListAsync();

                requestedProperties = await rentContext.Properties
                    .Where(p => requestedPropertyIds.Contains(p.Id))
                    .ToListAsync();

                if (!string.IsNullOrEmpty(searchString))
                {
                    searchString = searchString.ToLower();

                    requestedProperties = requestedProperties.Where(p =>
                        p.Address.ToLower().Contains(searchString) ||
                        p.City.ToLower().Contains(searchString) ||
                        p.Bedroom.ToString().Contains(searchString) ||
                        p.Bathroom.ToString().Contains(searchString) ||
                        p.Area.ToString().Contains(searchString) ||
                        p.PricePerMonth.ToString().Contains(searchString)
                    )
                    .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving requested properties: " + ex.Message);
            }

            return requestedProperties;
        }


        // Get Applicant List
        public async Task<List<AppUser>> GetApplicants(int propertyId)
        {
            List<AppUser> applicants = new List<AppUser>();

            try
            {
                var applicantIds = await rentContext.RentalRequests
                    .Where(rp => rp.PropertyId == propertyId)
                    .Select(rp => rp.TenantId)
                    .ToListAsync();

                applicants = await rentContext.Users
                    .Where(u => applicantIds.Contains(u.Id))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving applicants: " + ex.Message);
            }

            return applicants;
        }
    
        
        // Accept Applicants request
        public async Task<Status> AcceptApplicantsRequest(int propertyId, string tenantId)
        {
            Status status = new Status();

            // Retrieve the rent request based on the propertyId and tenantId
            RentRequests rentRequest = await rentContext.RentalRequests
                .Include(rr => rr.Property)
                .Include(rr => rr.Tenant)
                .FirstOrDefaultAsync(rr => rr.PropertyId == propertyId && rr.TenantId == tenantId);

            // Rent request not found
            if (rentRequest == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Rent request was not found!";
                return status;
            }

            // Check if the rent request is already accepted or rejected
            if (rentRequest.Status == "Accepted" || rentRequest.Status == "Rejected")
            {
                status.StatusCode = 0;
                status.StatusMessage = "Rent request is already processed.";
                return status;
            }

            // Reject all other requests for the same property
            List<RentRequests> otherRequests = await rentContext.RentalRequests
                .Where(rr => rr.PropertyId == propertyId && rr.TenantId != tenantId && rr.Status != "Rejected")
                .ToListAsync();

            foreach (var request in otherRequests)
                request.Status = "Rejected";

            // Update the rent request status to "Accepted"
            rentRequest.Status = "Accepted";

            try
            {
                // Save the changes to the database
                await rentContext.SaveChangesAsync();

                // Update the property's tenantId
                Property property = await rentContext.Properties.FindAsync(propertyId);
                property.TenantId = tenantId;
                property.IsRented = true;
                property.RentedDate = DateTime.Now;
                await rentContext.SaveChangesAsync();
                
                status.StatusCode = 1;
                status.StatusMessage = "Rent request accepted successfully!";
            }
            catch (Exception ex)
            {
                // Handle the exception if there is an error while saving changes
                status.StatusCode = 0;
                status.StatusMessage = "An error occurred while accepting the rent request: " + ex.Message;
            }

            return status;
        }


        // Reverting the accepted request of the applicant
        public async Task<Status> UndoApplicantsRequest(int propertyId, string tenantId)
        {
            Status status = new Status();

            // Retrieve the rent request based on the propertyId and tenantId
            RentRequests rentRequest = await rentContext.RentalRequests
                .Include(rr => rr.Property)
                .Include(rr => rr.Tenant)
                .FirstOrDefaultAsync(rr => rr.PropertyId == propertyId && rr.TenantId == tenantId);

            // Rent request not found
            if (rentRequest == null)
            {
                status.StatusCode = 0;
                status.StatusMessage = "Rent request was not found!";
                return status;
            }

            // Change all other requests for the same property to "Waiting"
            List<RentRequests> otherRequests = await rentContext.RentalRequests
                .Where(rr => rr.PropertyId == propertyId && rr.TenantId != tenantId && rr.Status == "Rejected")
                .ToListAsync();

            foreach (var request in otherRequests)
                request.Status = "Waiting";

            // Update the rent request status to "Waiting"
            rentRequest.Status = "Waiting";

            try
            {
                // Save the changes to the database
                await rentContext.SaveChangesAsync();

                // Update the property's tenantId
                Property property = await rentContext.Properties.FindAsync(propertyId);
                property.TenantId = tenantId;
                property.IsRented = false;
                property.RentedDate = null;
                await rentContext.SaveChangesAsync();
                
                status.StatusCode = 1;
                status.StatusMessage = "Undoing rent request successful!";
            }
            catch (Exception ex)
            {
                // Handle the exception if there is an error while saving changes
                status.StatusCode = 0;
                status.StatusMessage = "An error occurred while accepting the rent request: " + ex.Message;
            }

            return status;
        }


        // Check if Property is accepted
        public async Task<RentRequests> IsAccepted(int propertyId, string tenantId)
        {
            var acceptedRequest = await rentContext.RentalRequests
                        .FirstOrDefaultAsync(ar => ar.PropertyId == propertyId && ar.TenantId == tenantId && ar.Status == "Accepted");

            return acceptedRequest;
        }

        // Check if Property is rejected
        public async Task<RentRequests> IsRejected(int propertyId, string tenantId)
        {
            var rejectedRequest = await rentContext.RentalRequests
                        .FirstOrDefaultAsync(rr => rr.PropertyId == propertyId && rr.TenantId == tenantId && rr.Status == "Rejected");

            return rejectedRequest;
        }
    }
}
