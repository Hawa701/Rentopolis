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

        // Save Property
        public async Task<Status> SaveOrUnsaveToSavedProperties(int propertyId, string tenantId)
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

    }
}
