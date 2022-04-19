using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.DataAccess.Enumerations;
using System.Collections.Generic;

namespace DeliciousFood.DataAccess.DataModels
{
    /// <summary>
    /// User description
    /// </summary>
    public class User : Entity
    {
        public virtual string Login { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string Name { get; set; }

        public virtual Policy PolicyMask { get; set; }


        public virtual List<UserDeliciousFood> DeliciousFoods { get; set; }
    }
}

