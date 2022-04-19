namespace DeliciousFood.DataAccess.DataModels.Base
{
    /// <summary>
    /// Base entity with identifier
    /// </summary>
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }
    }
}