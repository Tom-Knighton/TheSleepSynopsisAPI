using System;
using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Domain.Services
{
    public interface ITokenService
    {
        public Task<UserAuthenticationTokens> GenerateInitialTokens(string userUUID);
        public Task<string?> CreateJWTForUser(string userUUID);
        public string GenerateByteString();
        public Task<UserAuthenticationTokens> RefreshTokensForUser(string userUUID, string refreshToken);
        public Task<bool> IsRefreshTokenValid(string userUUID, string refreshToken);
        public Task InvalidateAllRefreshTokens(string userUUID);
        public Task<bool> VerifyPassHash(string userUUID, string verifyingPass);
        public Tuple<string, string> NewHashAndSalt(string password);
    }
}

