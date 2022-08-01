using System;
using Microsoft.EntityFrameworkCore;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context) => _context = context;

        public async Task<ICollection<User>> GetAllUsers(bool includeDeleted = false)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => includeDeleted == false ? u.DeletedAt == null : true)
                .ToListAsync();
        }

        public async Task<User?> GetUser(string userUUID)
        {
            return await _context.Users
                .Where(u => u.UserUUID == userUUID)
                .Include(u => u.UserAuth)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<User>> SearchByName(string name, bool matchExactly = false)
        {
            name = name.ToLower();
            return await _context.Users
                .Where(u => (u.UserName.ToLower() == name || (!matchExactly && name.Length > 3 && u.UserName.Contains(name))))
                .AsNoTracking()
                .Take(10)
                .ToListAsync();
        }
    }
}

