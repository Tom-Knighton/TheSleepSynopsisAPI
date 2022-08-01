using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class FeedPost
    {
        public string PostUUID { get; set; }
        public string UserUUID { get; set; }
        public string? PostDreamUUID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string PostText { get; set; }
    }
}

