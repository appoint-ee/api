namespace api.Models;

public class TimeSlot
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string? Status { get; init; }
}