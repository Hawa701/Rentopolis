using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;
using Rentopolis.Repositories.Interfaces;
using System.Net;

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

            Property newProperty = new Property
            {
                ImageUrl = "/Images/Property Images/default.png",
                Address = model.Address,
                City = model.City,
                Features = model.Features,
                Bedroom = model.Bedroom,
                Bathroom = model.Bedroom,
                Area = model.Area,
                PricePerMonth = model.PricePerMonth,
                LandlordId = model.LandlordId,
                IsApproved = false,
                IsDeleted = false,
                IsRented = false,
                AddedDate = DateTime.Now,
            };

            try
            {
                rentContext.Properties.Add(newProperty);
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


        // List all properties
        public async Task<List<Property>> GetAllProperties()
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.ToListAsync();
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
        public async Task<List<Property>> GetOwnedProperties(string id)
        {
            List<Property> properties = new List<Property>();

            try
            {
                properties = await rentContext.Properties.Where(p => p.LandlordId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving properties: " + ex.Message);
            }

            return properties;
        }


        // List approved properties
        public async Task<List<Property>> GetApprovedProperties()
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

            return properties;
        }


        // Get property detail
        public async Task<Property> GetPropertyDetail(int id)
        {
            Property property = new Property();

            try
            {
                property = await rentContext.Properties.FindAsync(id);
                //property.Id = prop.Id;
                //property.ImageUrl = prop.ImageUrl;
                //property.Address = prop.Address;
                //property.City = prop.City;
                //property.Features = prop.Features;
                //property.Bedroom = prop.Bedroom;
                //property.Bathroom = prop.Bathroom;
                //property.Area = prop.Area;
                //property.PricePerMonth = prop.PricePerMonth;
                //property.LandlordId = prop.LandlordId;
                //property.IsApproved = prop.IsApproved;
                //property.IsDeleted = prop.IsDeleted;
                //property.IsRented = prop.IsRented;
                //property.AddedDate = prop.AddedDate;
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
            property.Bathroom = model.Bathroom;
            property.Bedroom = model.Bedroom;
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
    }
}
