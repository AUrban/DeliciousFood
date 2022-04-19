using DeliciousFood.DataAccess.DataModels.Base;
using System;

namespace DeliciousFood.DataAccess.DataModels
{
    /// <summary>
    /// Refresh token description
    /// Used to change the authentication access token
    /// </summary>
    public class RefreshToken : Entity
    {
        public virtual string Token { get; set; }

        public virtual int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual DateTime LifeTime { get; set; }

        public virtual DateTime CreateTime { get; set; }
    }
}
