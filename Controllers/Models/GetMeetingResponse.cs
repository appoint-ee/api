namespace api.Controllers.Models;

public class GetMeetingResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }
    
    public string Status { get; init; }

    public DateTime StartTime { get; init; }

    public DateTime EndTime { get; init; }
}