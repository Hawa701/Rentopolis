using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IPropertyServices
    {
        Task<Status> AddNewProperty(NewPropertyViewModel model);
        Task<List<Property>> GetAllProperties();
        Task<List<Property>> GetNonApprovedProperties();
        Task<List<Property>> GetOwnedProperties(string id);

        Task<List<Property>> GetApprovedProperties();
    }
}
