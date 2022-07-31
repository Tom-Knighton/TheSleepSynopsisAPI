using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class Dream
    {
        public string DreamUUID { get; set; }
        public string? SleepEntryUUID { get; set; }
        public string UserUUID { get; set; }
        public string DreamTitle { get; set; }
        public string DreamText { get; set; }
    }
}

