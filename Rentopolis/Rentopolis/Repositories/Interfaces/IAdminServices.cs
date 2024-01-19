using Microsoft.AspNetCore.Identity;
using Rentopolis.Models.Data;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAdminServices
    {
        Task<List<AppUser>> GetUsersByRole(string role);
    }
}
