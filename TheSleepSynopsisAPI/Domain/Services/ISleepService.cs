using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Domain.Services
{
    public interface ISleepService
    {
        public Task<SleepEntry?> GetSleepEntry(string sleepUUID);
        public Task<SleepEntry?> CreateSleepEntry(SleepEntry sleepEntry, string? overrideUserUUID = null);
        public Task<ICollection<SleepEntry>> GetSleepEntriesForUser(string userUUID);

        public Task<Dream?> GetDream(string dreamUUID);
        public Task<Dream?> ModifyDream(EditDreamDTO dto);
        
    }
}

