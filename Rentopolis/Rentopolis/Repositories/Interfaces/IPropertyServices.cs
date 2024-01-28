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
        Task<Property> GetPropertyDetail(int id);
        Task<Status> ApproveProperty(int id);
        Task<Status> RejectProperty(int id);
        Task<UpdatePropertyInfoViewModel> GetPropertyDetailForEditing(int id);
        Task<Status> EditPropertyInfo(UpdatePropertyInfoViewModel model);
        Task<Status> DeletePropertyInfo(int id);
    }
}
