using DeliciousFood.DataAccess.Enumerations;

namespace DeliciousFood.Services.Security
{
    /// <summary>
    /// A provider to get the current logged user parameters
    /// </summary>
    public interface IUserSessionProvider
    {
        int UserId { get; }

        Policy PolicyMask { get; }
    }
}
