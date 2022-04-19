using System;

namespace DeliciousFood.Services.Exceptions
{
    /// <summary>
    /// A basic service exception
    /// Used to catch service errors and return http codes 
    /// </summary>
    public abstract class ServiceException : Exception
    {
        public ServiceException()
            : base()
        {
        }

        /// <summary>
        /// Error keyword
        /// </summary>
        public abstract string KeyError { get; }

        /// <summary>
        /// Error description
        /// </summary>
        public abstract string Error { get; }
    }
}
