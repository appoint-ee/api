using api.Data.Enums;

namespace api.Controllers.Models;

public class UpdateMeetingRequest
{
    public Guid Id { get; init; }

    public long AttendeeId { get; set; }

    public AttendeeStatus Status { get; init; }
}