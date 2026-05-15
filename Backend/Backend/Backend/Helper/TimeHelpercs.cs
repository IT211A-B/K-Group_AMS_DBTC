namespace Backend.Backend.Helper
{
    /// <summary>
    /// Utility for Manila timezone conversions.
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// Gets current Manila time from UTC.
        /// </summary>
        public static DateTime Now()
        {
            TimeZoneInfo manilaZone =
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, manilaZone);
        }
    }
}
