﻿using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IAccountServices
    {
        Task<FullInfoViewModel> GetUserByIdForView(string id);
        Task<UpdateUserInfoViewModel> GetUserByIdForEdit(string id);
        Task<Status> LoginUser(LoginViewModel model);
        Task LogoutUser();
        Task<Status> RegisterUser(RegisterationViewModel model);
        Task<Status> EditUserProfile(UpdateUserInfoViewModel model);
        Task<Status> DeleteUserProfile(string id);
    }
}
