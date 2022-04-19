namespace DeliciousFood.Services.Base.Model
{
    /// <summary>
    /// Interface for edit models with an identifier
    /// </summary>
    public interface IIdentifiedEditModel : IEditModel
    {
        int? Id { get; set; }
    }
}
