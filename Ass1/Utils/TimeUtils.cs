namespace Ass1.Utils
{
    public class TimeUtils
    {
        public static DateTime asiaTime()
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        }
    }
}
