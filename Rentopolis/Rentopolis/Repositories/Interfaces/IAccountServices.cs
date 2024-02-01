using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAccountServices
    {
        Task<FullInfoViewModel> GetUserByIdForView(string id);
        Task<UpdateUserInfoViewModel> GetUserByIdForEdit(string id);
        Task<PasswordViewModel> GetUserByIdForPassword(string id);
        Task<Status> LoginUser(LoginViewModel model);
        Task LogoutUser();
        Task<Status> RegisterUser(RegisterationViewModel model);
        Task<Status> EditUserProfile(UpdateUserInfoViewModel model);
        Task<Status> ChangeUserPassword(PasswordViewModel model);
        Task<Status> DeleteUserProfile(string id);
        Task<Status> ReportUser(string id, string message);
        //Task<int> GetReportCountByUserId(string userId);
        Task<List<ReportedUsers>> GetReportsByUserId(string userId);
    }
}
