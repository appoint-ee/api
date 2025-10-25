using api.Data.Enums;

namespace api.Controllers.Models;

public class UpdateMeetingRequest
{
    public long AttendeeId { get; set; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public decimal? Price { get; init; }
    public AttendeeStatus? Status { get; init; }
}