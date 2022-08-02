using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class Dream
    {
        public string DreamUUID { get; set; }
        public string? SleepEntryUUID { get; set; }
        public string DreamTitle { get; set; }
        public string DreamText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public virtual SleepEntry SleepEntry { get; set; }
    }

    public class EditDreamDTO
    {
        public string DreamUUID { get; set; }
        public string DreamTitle { get; set; }
        public string DreamText { get; set; }
    }
}

