using api.Data.Enums;

namespace api.Services.Dtos;

public class AttendeeDto
{
    public Guid Id { get; init; }

    public long? UserId { get; init; }

    public bool IsHost { get; init; }

    public Guid MeetingId { get; init; }
    
    public AttendeeStatus Status { get; init; }
}