namespace DeliciousFood.Services.Base.Model
{
    public abstract class IdentifiedEntityViewModel : IIdentifiedViewModel
    {
        public virtual int Id { get; set; }
    }
}
