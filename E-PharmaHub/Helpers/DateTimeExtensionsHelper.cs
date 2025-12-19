namespace E_PharmaHub.Helpers
{
    public static class DateTimeExtensionsHelper
    {
        public static DateTime ToEgyptTime(this DateTime utcDateTime)
        {
            var egyptTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, egyptTimeZone);
        }
    }

}
