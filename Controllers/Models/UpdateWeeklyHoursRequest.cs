namespace api.Controllers.Models;

public class UpdateWeeklyHoursRequest
{
    public int DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
}