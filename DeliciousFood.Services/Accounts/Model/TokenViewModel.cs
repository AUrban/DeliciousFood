namespace DeliciousFood.Services.Accounts.Model
{
    /// <summary>
    /// View model for getting auth token
    /// </summary>
    public class TokenViewModel
    {
        /// <summary>
        /// Access token for each request
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Token for updating access token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
