using System;

namespace DeliciousFood.DataAccess.Enumerations
{
    /// <summary> Permissions </summary>
    [Flags]
    public enum Policy
    {
        /// <summary>
        /// None value
        /// </summary>
        None,

        /// <summary> CRUD own food records </summary>
        UsersPolicy = 1,

        /// <summary> CRUD users only </summary>
        ModeratorsPolicy = 2,

        /// <summary> CRUD any entities </summary>
        AdminsPolicy = 4
    }
}
