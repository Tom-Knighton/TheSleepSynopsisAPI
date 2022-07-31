using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class FeedPostReply
    {
        public string PostReplyUUID { get; set; }
        public string PostUUID { get; set; }
        public string UserUUID { get; set; }
        public string ReplyText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}

