using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class UserAuth
    {
        public string UserUUID { get; set; }
        public string UserEmail { get; set; }
        public string UserPassHash { get; set; }
        public string UserPassSalt { get; set; }

        public virtual User User { get; set; }
    }

    public class UserAuthenticationTokens
    {
        public string AuthenticationToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class UserRefreshToken
    {
        public string UserUUID { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenIssueDate { get; set; }
        public DateTime? TokenExpiryDate { get; set; }
        public string TokenClient { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User User { get; set; }
    }
}

