﻿using Rentopolis.Models.Data;
using Rentopolis.Models.Entitiy;

namespace Rentopolis.Repositories.Interfaces
{
    public interface IManagerServices
    {
        Task<List<AppUser>> GetUsersByRole(string role);
        Task<Status> BanUser(string id);
        Task<Status> UnbanUser(string id);
    }
}