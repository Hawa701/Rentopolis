using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAdminServices
    {
        Task<List<AppUser>> GetUsersByRole(string role, string searchString);
        Task<Status> CreateNewManager(RegisterationViewModel model);
        Task<Status> DeleteManager(string id);
    }
}
