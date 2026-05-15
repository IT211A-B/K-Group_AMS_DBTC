namespace Backend.Backend.Configuration
{
    public class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        public int AuthPermitLimit { get; set; } = 10;
        public int AuthWindowSeconds { get; set; } = 60;
        public int ApiPermitLimit { get; set; } = 120;
        public int ApiWindowSeconds { get; set; } = 60;
    }
}
