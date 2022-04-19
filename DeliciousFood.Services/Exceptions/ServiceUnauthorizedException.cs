namespace DeliciousFood.Services.Exceptions
{
    /// <summary>
    /// This exception is thrown when the model fails authorization in the service.
    /// 401 error should be returned.
    /// </summary>
    public class ServiceUnauthorizedException : ServiceException
    {
        public override string KeyError => null;

        public override string Error => null;

        public ServiceUnauthorizedException()
        {
        }
    }
}
