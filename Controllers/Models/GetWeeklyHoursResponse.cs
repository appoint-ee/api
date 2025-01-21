namespace api.Controllers.Models;

public class GetWeeklyHoursResponse
{
    public Guid Id { get; init; }
 
    public long UserId { get; init; }
    
    public int DayOfWeek { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}