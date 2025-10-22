namespace api.Controllers.Models;

public class GetMeetingResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }
    
    public string Status { get; init; }
    
    public long ServiceId { get; init; }

    public string ServiceName { get; init; }

    public TimeSpan Duration { get; init; }
    
    public decimal Price { get; init; }

    public DateTime StartTime { get; init; }

    public DateTime EndTime { get; init; }
}