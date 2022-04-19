namespace DeliciousFood.Services.Base.Model
{
    /// <summary>
    /// Interface for view models with an identifier
    /// </summary>
    public interface IIdentifiedViewModel : IViewModel
    {
        int Id { get; set; }
    }
}
