using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.DataAccess.Enumerations;
using System.Collections.Generic;

namespace DeliciousFood.DataAccess.DataModels
{
    /// <summary>
    /// Food description
    /// </summary>
    public class Food : Entity, ISubEntity<User>
    {
        public virtual int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual string Title { get; set; }

        public virtual FoodType Type { get; set; }

        public virtual decimal NumberOfCalories { get; set; }

        public virtual string Country { get; set; }

        public virtual bool IsPublic { get; set; }


        public virtual List<UserDeliciousFood> DeliciousFoods { get; set; }

        User ISubEntity<User>.Parent
        {
            get => User;
            set 
            {
                User = value;
                UserId = value.Id;
            }
        }
    }
}
