using System;
using System.Collections.Generic;
using System.Net;
using DeliciousFood.Common.Helpers;

namespace DeliciousFood.Services.Exceptions
{
    /// <summary>
    /// Exceptions handling by binding with the corresponding http codes
    /// </summary>
    public class ServiceExceptionHandlerOptions
    {
        private IDictionary<Type, HttpStatusCode> MapExceptionToStatusCode { get; set; }

        public ServiceExceptionHandlerOptions()
        {
            MapExceptionToStatusCode = new Dictionary<Type, HttpStatusCode>();
        }

        /// <summary>
        /// Bind the service exception with the corresponding http code
        /// </summary>
        public void Bind(Type serviceExceptionType, HttpStatusCode statusCode)
        {
            if (!serviceExceptionType.BasedOn<ServiceException>())
                throw new ArgumentException("Attempt to bind not service exception");

            if (!MapExceptionToStatusCode.ContainsKey(serviceExceptionType))
                MapExceptionToStatusCode.Add(serviceExceptionType, statusCode);
            else
                MapExceptionToStatusCode[serviceExceptionType] = statusCode;
        }

        public bool Contains(Type type)
        {
            return MapExceptionToStatusCode.ContainsKey(type);
        }

        /// <summary>
        /// Get status code by exception type
        /// </summary>
        public HttpStatusCode GetHttpStatusCode(Type type)
        {
            if (Contains(type))
                return MapExceptionToStatusCode[type];

            throw new ArgumentException("Attempt to get http code for unbinded service exception");
        }
    }
}
