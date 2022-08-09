using System;
using Microsoft.EntityFrameworkCore;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly DataContext _context;

        public AuthService(ITokenService tokens, IUserService users, DataContext context)
        {
            _tokenService = tokens;
            _context = context;
            _userService = users;
        }

        public async Task<User?> Authenticate(UserAuthRequest request, bool needsTokens = false)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == request.UserAuthString || u.UserAuth.UserEmail.ToLower() == request.UserAuthString.ToLower());
            if (user == null)
            {
                Console.WriteLine($"LOG: Invalid email/username attempted: {request.UserAuthString}");
                throw new AuthenticationException("Invalid email/username or password");
            }

            if (await _tokenService.VerifyPassHash(user.UserUUID, request.UserPass) == false)
            {
                Console.WriteLine($"LOG: Invalid pass attempt for {user.UserUUID}");
                throw new AuthenticationException("Invalid email/username or password");
            }

            user = await _userService.GetUser(user.UserUUID);
            if (user != null)
            {
                user.AuthTokens = needsTokens ? await _tokenService.GenerateInitialTokens(user.UserUUID) : null;
                return user;
            }

            Console.WriteLine($"LOG: Weird error, user null? logging in");
            throw new AuthenticationException("An unknown error occurred logging in.");
        }

        public async Task<User?> CreateUser(NewUserDTO dto)
        {
            if (await IsEmailFree(dto.UserEmail) == false)
                throw new RegistrationException("Email Address is already in use");

            if (await IsUsernameFree(dto.UserName) == false)
                throw new RegistrationException("Username is already in use");

            string newUUID = Guid.NewGuid().ToString("N");
            Tuple<string, string> hashedPass = _tokenService.NewHashAndSalt(dto.UserPassword);
            User user = new()
            {
                UserUUID = newUUID,
                UserName = dto.UserName,
                UserFullName = dto.UserFullName,
                UserRole = UserRole.user,
                UserAuth = new()
                {
                    UserEmail = dto.UserEmail,
                    UserPassHash = hashedPass.Item1,
                    UserPassSalt = hashedPass.Item2
                }
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            user = await _userService.GetUser(user.UserUUID);
            user.AuthTokens = await _tokenService.GenerateInitialTokens(user.UserUUID);

            return user;
        }

        public async Task<bool> IsEmailFree(string email)
        {
            email = email.ToLower();
            return await _context.Users
                .Include(u => u.UserAuth)
                .Where(u => u.UserAuth.UserEmail.ToLower() == email && u.DeletedAt == null)
                .Select(u => u.UserAuth.UserEmail)
                .FirstOrDefaultAsync() == null;
        }

        public async Task<bool> IsUsernameFree(string username)
        {
            username = username.ToLower();
            return await _context.Users
                .Where(u => u.UserName.ToLower() == username && u.DeletedAt == null)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync() == null;
        }
    }
}

