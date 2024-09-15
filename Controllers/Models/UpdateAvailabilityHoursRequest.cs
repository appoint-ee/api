namespace api.Controllers.Models;

public class UpdateAvailabilityHoursRequest
{
    public int DayOfWeek { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
}