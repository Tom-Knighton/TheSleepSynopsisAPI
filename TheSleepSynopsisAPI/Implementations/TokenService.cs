using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Implementations
{
    public class TokenService : ITokenService
    {
        private DataContext _context;
        private AppSettings _settings;

        public TokenService(DataContext context, IOptions<AppSettings> settings)
        {
            _context = context;
            _settings = settings.Value;
        }

        public async Task<string?> CreateJWTForUser(string userUUID)
        {
            User? user = await _context.Users.FindAsync(userUUID);
            if (user == null)
                return null;

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] secretKey = Encoding.ASCII.GetBytes(_settings.SecretKey);
            List<Claim> claims = new() { new Claim(ClaimTypes.Name, user.UserUUID), new Claim(ClaimTypes.Role, user.UserRole.ToString()) };

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow
            };

            SecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateByteString()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<UserAuthenticationTokens> GenerateInitialTokens(string userUUID)
        {
            string initialRefreshTokenString = GenerateByteString();
            UserRefreshToken refreshToken = new()
            {
                RefreshToken = initialRefreshTokenString,
                UserUUID = userUUID,
                TokenIssueDate = DateTime.UtcNow,
                TokenExpiryDate = DateTime.UtcNow.AddMonths(2),
                TokenClient = "N/A",
                IsDeleted = false
            };

            await _context.UserRefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return new UserAuthenticationTokens
            {
                AuthenticationToken = await CreateJWTForUser(userUUID) ?? "",
                RefreshToken = initialRefreshTokenString
            };
        }

        public async Task InvalidateAllRefreshTokens(string userUUID)
        {
            List<UserRefreshToken> tokens = await _context.UserRefreshTokens.AsNoTracking().Where(t => t.UserUUID == userUUID).ToListAsync();
            tokens.ForEach(t =>
            {
                t.IsDeleted = true;
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsRefreshTokenValid(string userUUID, string refreshToken)
        {
            UserRefreshToken? token = await _context.UserRefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserUUID == userUUID && t.RefreshToken == refreshToken && !t.IsDeleted);
            if (token != null)
            {
                return token.TokenExpiryDate > DateTime.UtcNow;
            }

            return false;
        }

        

        public async Task<UserAuthenticationTokens> RefreshTokensForUser(string userUUID, string refreshToken)
        {
            if (await IsRefreshTokenValid(userUUID, refreshToken) == false)
                throw new RefreshException("Supplied token was invalid");

            UserRefreshToken? oldToken = await _context.UserRefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.UserUUID == userUUID);

            if (oldToken == null)
                throw new RefreshException("Token was not found");

            oldToken.IsDeleted = true;

            string newRefreshTokenString = GenerateByteString();
            UserRefreshToken newToken = new()
            {
                UserUUID = userUUID,
                RefreshToken = newRefreshTokenString,
                TokenClient = "N/A",
                TokenIssueDate = DateTime.UtcNow,
                IsDeleted = false,
                TokenExpiryDate = DateTime.UtcNow.AddMonths(2),
            };
            await _context.UserRefreshTokens.AddAsync(newToken);
            _context.UserRefreshTokens.Update(oldToken);
            await _context.SaveChangesAsync();

            return new()
            {
                AuthenticationToken = await CreateJWTForUser(userUUID),
                RefreshToken = newRefreshTokenString
            };
        }

        public Tuple<string, string> NewHashAndSalt(string password)
        {
            string salt = GenerateByteString();
            Rfc2898DeriveBytes pbkdf2 = new(password, Convert.FromBase64String(salt), iterations: 4096, HashAlgorithmName.SHA256);
            return new Tuple<string, string>(Convert.ToBase64String(pbkdf2.GetBytes(64)), salt);
        }

        public async Task<bool> VerifyPassHash(string userUUID, string verifyingPass)
        {
            User? user = await _context.Users
                .Include(u => u.UserAuth)
                .FirstOrDefaultAsync(u => u.UserUUID == userUUID);
            if (user != null)
            {
                Rfc2898DeriveBytes pbkdf2 = new(verifyingPass, Convert.FromBase64String(user.UserAuth.UserPassSalt), iterations: 4096, HashAlgorithmName.SHA256);
                return Convert.ToBase64String(pbkdf2.GetBytes(64)).Equals(user.UserAuth.UserPassHash);
            }

            return false;
        }
    }
}

