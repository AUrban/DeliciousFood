namespace DeliciousFood.DataAccess.DataModels.Base
{
    /// <summary>
    /// Interface for the child entity with identifier
    /// </summary>
    public interface ISubEntity<TParent> where TParent : class, IEntity
    {
        TParent Parent { get; set; }
    }
}
