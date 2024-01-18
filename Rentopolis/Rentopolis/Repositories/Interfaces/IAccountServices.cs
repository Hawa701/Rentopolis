using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAccountServices
    {
        Task<Update> GetUserById(string id);
        Task<Status> LoginAsync(Login model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(Registeration model);
        Task<Status> EditUserProfile(Update model);
    }
}
