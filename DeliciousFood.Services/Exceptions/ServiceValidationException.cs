namespace DeliciousFood.Services.Exceptions
{
    /// <summary>
    /// This exception is thrown when the model does not pass an additional validation in the service.
    /// </summary>
    public class ServiceValidationException : ServiceException
    {
        public override string KeyError { get; }

        public override string Error { get; }

        public ServiceValidationException()
        {
        }

        public ServiceValidationException(string error) : this("error", error)
        {
        }

        public ServiceValidationException(string key, string error)
        {
            KeyError = key;
            Error = error;
        }
    }
}
