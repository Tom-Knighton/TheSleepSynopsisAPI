using System;
namespace TheSleepSynopsisAPI.Domain.Models
{
    public class User
    {
        public string UserUUID { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public UserRole UserRole { get; set; }
    }

    public enum UserRole
    {
        user, admin
    }
}

