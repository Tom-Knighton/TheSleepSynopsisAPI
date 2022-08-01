using System;
using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Domain.Services
{
    public interface IUserService
    {
        public Task<ICollection<User>> GetAllUsers(bool includeDeleted = false);
        public Task<User?> GetUser(string userUUID);
        public Task<ICollection<User>> SearchByName(string name, bool matchExactly = false);
    }
}

