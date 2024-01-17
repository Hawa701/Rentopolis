using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IUserAuthenticationServices
    {
        Task<Status> LoginAsync(Login model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(Registeration model);
        Task<string> GetUserRoleAsync(string username);
    }
}
