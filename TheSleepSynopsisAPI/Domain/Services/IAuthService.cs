using System;
using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Domain.Services
{
    public interface IAuthService
    {
        Task<User?> Authenticate(UserAuthRequest request, bool needsTokens = false);
        Task<User?> CreateUser(NewUserDTO dto);
        Task<bool> IsEmailFree(string email);
        Task<bool> IsUsernameFree(string username);
    }
}

