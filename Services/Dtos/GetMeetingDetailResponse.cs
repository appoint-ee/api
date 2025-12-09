namespace api.Services.Dtos;

public class GetMeetingDetailResponse
{
    public Guid Id { get; init; }

    public string Title { get; init; }

    public string Description { get; init; }
    
    public string ServiceName { get; init; }

    public TimeSpan Duration { get; set; }
    
    public decimal Price { get; set; }

    public DateTimeOffset StartTimeUtc { get; init; }

    public DateTimeOffset EndTimeUtc { get; init; }
    
    public ICollection<AttendeeDto> Attendees { get; set; }
}