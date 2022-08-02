using System;
using Microsoft.EntityFrameworkCore;
using TheSleepSynopsisAPI.Data;
using TheSleepSynopsisAPI.Domain.Models;
using TheSleepSynopsisAPI.Domain.Services;

namespace TheSleepSynopsisAPI.Implementations
{
    public class SleepService : ISleepService
    {
        private readonly DataContext _context;

        public SleepService(DataContext context)
        {
            _context = context;
        }

        public async Task<SleepEntry?> GetSleepEntry(string sleepUUID)
        {
            return await _context.SleepEntries
                .AsNoTracking()
                .Where(s => s.SleepUUID == sleepUUID)
                .Include(s => s.Dreams)
                .FirstOrDefaultAsync();
        }

        public async Task<SleepEntry?> CreateSleepEntry(SleepEntry sleepEntry, string? overrideUserUUID = null)
        {
            string newGUID = Guid.NewGuid().ToString("N");
            sleepEntry.SleepUUID = newGUID;
            sleepEntry.UserUUID = overrideUserUUID ?? sleepEntry.UserUUID;

            foreach (Dream dream in sleepEntry.Dreams)
            {
                dream.DreamUUID = Guid.NewGuid().ToString("N");
                dream.CreatedAt = DateTime.UtcNow;
            }

            await _context.SleepEntries.AddAsync(sleepEntry);
            await _context.SaveChangesAsync();
            return await GetSleepEntry(newGUID);
        }

        public async Task<ICollection<SleepEntry>> GetSleepEntriesForUser(string userUUID)
        {
            return await _context.SleepEntries
                .AsNoTracking()
                .Where(s => s.UserUUID == userUUID)
                .Include(s => s.Dreams)
                .ToListAsync();
        }

        public async Task<Dream?> GetDream(string dreamUUID)
        {
            return await _context.Dreams
                .Where(d => d.DreamUUID == dreamUUID)
                .FirstOrDefaultAsync();
        }

        public async Task<Dream?> ModifyDream(EditDreamDTO dto)
        {
            Dream? oldDream = await _context.Dreams
                .AsNoTracking()
                .Where(d => d.DreamUUID == dto.DreamUUID)
                .FirstOrDefaultAsync();

            if (oldDream == null)
                return null;

            oldDream.DreamTitle = dto.DreamTitle;
            oldDream.DreamText = dto.DreamText;
            oldDream.ModifiedAt = DateTime.UtcNow;

            _context.Dreams.Update(oldDream);
            await _context.SaveChangesAsync();
            return await GetDream(dto.DreamUUID);
        }
    }
}

