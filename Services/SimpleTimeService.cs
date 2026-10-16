namespace TriangleLab.Services;

public class SimpleTimeService : ITimeService
{
    public DateTime GetTimeForTomorrow()
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv"));
        return now.AddDays(1);
    }
}