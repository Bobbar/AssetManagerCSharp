using System;

namespace AssetManager.Security
{
    public class InvalidAccessException : Exception
    {
        public InvalidAccessException() { }

        public InvalidAccessException(string message) : base(message) { }

        public InvalidAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
