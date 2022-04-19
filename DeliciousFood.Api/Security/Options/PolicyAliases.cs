namespace DeliciousFood.Api.Security.Options
{
    /// <summary>
    /// List of .net core authorization policies
    /// </summary>
    public class PolicyAliases
    {
        /// <summary>
        /// Permission only for users or admins
        /// </summary>
        public const string UsersAdminsPolicy = "UsersAdminsPolicy";

        /// <summary>
        /// Permission only for moderators or admins
        /// </summary>
        public const string ModeratorsAdminsPolicy = "ModeratorsAdminsPolicy";
    }
}
