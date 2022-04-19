using DeliciousFood.DataAccess.DataModels.Base;

namespace DeliciousFood.DataAccess.DataModels
{
    /// <summary>
    /// Description of user's delicious food
    /// </summary>
    public class UserDeliciousFood : Entity, ISubEntity<User>, ISubEntity<Food>
    {
        public virtual int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual int FoodId { get; set; }

        public virtual Food Food { get; set; }


        User ISubEntity<User>.Parent
        {
            get => User;
            set
            {
                User = value;
                UserId = value.Id;
            }
        }

        Food ISubEntity<Food>.Parent
        {
            get => Food;
            set
            {
                Food = value;
                FoodId = value.Id;
            }
        }
    }
}

