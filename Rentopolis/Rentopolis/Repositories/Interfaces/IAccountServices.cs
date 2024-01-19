using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAccountServices
    {
        Task<FullInfoViewModel> GetUserById(string id);
        Task<Status> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegisterationViewModel model);
        Task<Status> EditUserProfile(FullInfoViewModel model);
        Task<Status> DeleteUserProfile(string id);
    }
}
