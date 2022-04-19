namespace DeliciousFood.Api.Settings
{
    /// <summary>
    /// POCO class for configuration settings    
    /// </summary>
    public class ConnectionStringsSettings
    {
        /// <summary>
        /// The name of the section with database configuration
        /// </summary>
        public string DbConfiguration { get; set; }

        /// <summary>
        /// A project name with database migrations
        /// </summary>
        public string MigrationProject { get; set; }
    }
}
