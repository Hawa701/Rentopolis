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
                properties = await rentContext.Properties.Where(p => p.IsApproved == false).ToListAsync();
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
    }
}
