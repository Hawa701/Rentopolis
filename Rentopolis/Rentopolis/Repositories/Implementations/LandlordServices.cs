using Microsoft.EntityFrameworkCore;
using Rentopolis.Models.Data;
using Rentopolis.Repositories.Interfaces;

namespace Rentopolis.Repositories.Implementations
{
    public class LandlordServices : ILandlordServices
    {
        private readonly RentContext rentContext;

        public LandlordServices(IWebHostEnvironment webHostEnvironment, RentContext rentContext)
        {
            this.rentContext = rentContext;
        }


        // Get number of properties (owned) based on the specified status
        public async Task<int> GetNumberOfOwnedPropertyStatus(string id, string neededStatus)
        {
            List<Property> propList = new List<Property>();

            try
            {
                propList = await rentContext.Properties.Where(p => p.LandlordId == id).ToListAsync();
                if (neededStatus == "Approved")
                {
                    propList = propList.Where(p => p.IsApproved == true && p.IsDeleted == false).ToList();
                }
                else if (neededStatus == "Rejected")
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
    }
}
