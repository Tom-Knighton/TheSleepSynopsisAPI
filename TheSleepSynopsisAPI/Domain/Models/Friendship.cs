using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class Friendship
    {
        public string InitiatorUUID { get; set; }
        public string SecondUUID { get; set; }
        public DateTime? DateAccepted { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBlocked { get; set; }

        public virtual User Initiator { get; set; }
        public virtual User SecondUser { get; set; }
    }
}

