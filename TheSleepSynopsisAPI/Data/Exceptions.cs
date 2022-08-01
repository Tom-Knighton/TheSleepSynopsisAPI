using System;
namespace TheSleepSynopsisAPI.Data
{
    public class AuthenticationException : Exception
    {
        public string description { get; private set; }

        public AuthenticationException(string message = "")
        {
            description = message;
        }
    }

    public class RegistrationException : Exception
    {
        public string description { get; private set; }

        public RegistrationException(string message = "")
        {
            description = message;
        }
    }

    public class RefreshException : Exception
    {
        public string description { get; private set; }

        public RefreshException(string message = "")
        {
            description = message;
        }
    }

}

