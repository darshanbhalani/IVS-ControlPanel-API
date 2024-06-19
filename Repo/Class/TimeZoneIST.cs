namespace IVS_API.Repo.Class
{
    public class TimeZoneIST
    {
        public static DateTime now()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }
    }
}
