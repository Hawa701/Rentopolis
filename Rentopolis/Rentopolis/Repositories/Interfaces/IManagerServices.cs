using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IManagerServices
    {
        Task<List<AppUser>> GetReportedUsersByRole(string role, string searchString);
        Task<Status> BanUser(string id);
        Task<Status> UnbanUser(string id);
        Task<int> GetNumberOfNonApprovedProperties();
        Task<int> NumberOfReports();
        Task<int> GetNumberOfBannedUsers();
        Task<int> GetNumberOfUnbannedUsers();
    }
}
