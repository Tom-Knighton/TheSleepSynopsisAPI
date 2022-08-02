using System;
using System.Security.Claims;
using TheSleepSynopsisAPI.Domain.Models;

namespace TheSleepSynopsisAPI.Utilities
{
    public static class AuthUtilities
    {
        public static string GetUUIDFromIdentity(ClaimsPrincipal identity)
        {
            return identity.Identity?.Name ?? "";
        }

        public static bool IsSameUser(ClaimsPrincipal identity, string userUUID)
        {
            return GetUUIDFromIdentity(identity).Equals(userUUID);
        }

        public static bool IsAdmin(ClaimsPrincipal identity)
        {
            return identity.HasClaim(ClaimTypes.Role, UserRole.admin.ToString());
        }

        public static bool IsSameUserOrAdmin(ClaimsPrincipal identity, string userUUID)
        {
            return IsSameUser(identity, userUUID) || IsAdmin(identity);
        } 
    }
}

