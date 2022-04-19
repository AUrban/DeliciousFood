using System;

namespace DeliciousFood.Services.Exceptions
{
    /// <summary>
    /// This exception is thrown when no object for the specified id is found in the service
    /// </summary>
    public class ServiceNotFoundException : ServiceException
    {
        public Type Entity { get; set; }

        /// <summary>
        /// A field name for seeking
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// A field value for seeking
        /// </summary>
        public object ParameterValue { get; set; }


        public override string KeyError => $"{Entity.Name}";

        public override string Error => $"Entity not found with property {ParameterName} = { ParameterValue }";


        public ServiceNotFoundException(Type entity) : this(entity, null, null)
        {
        }

        public ServiceNotFoundException(Type entity, string parameterName, object parameterValue)
        {
            Entity = entity;
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }
    }
}
