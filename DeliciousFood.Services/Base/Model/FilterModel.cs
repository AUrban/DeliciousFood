namespace DeliciousFood.Services.Base.Model
{
    /// <summary>
    /// A request parameter model for filtering records for endpoints getting all records
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// A filter string with 'or', 'and', brackets and various comparison operations
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// A skip parameter for paging
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// A limit parameter for paging
        /// </summary>
        public int? Limit { get; set; }
    }
}
