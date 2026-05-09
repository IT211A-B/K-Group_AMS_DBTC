namespace Backend.Backend.Configuration
{
    public class ConfigurationModel
    {
        /// <summary>
        /// Secret key used to sign JWT tokens
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Issuer of the JWT token, the app 
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// Audience for which the JWT token is valid (eg. ams-student)
        /// </summary>
        public required string Audience { get; set; }

        /// <summary>
        /// Expiration time in minutes.
        /// </summary>
        public int ExpireMinutes { get; set; }

    }
}