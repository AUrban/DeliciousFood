using System;

namespace DeliciousFood.DataAccess.Exceptions
{
    /// <summary>
    /// Repository permission operation exception
    /// </summary>
    public class DataAccessPermissionException : Exception
    {
        public DataAccessPermissionException(string message)
            : base(message)
        {
        }
    }
}
