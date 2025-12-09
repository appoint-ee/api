namespace api.Services.Dtos;

public record GetMeetingRequest(
    DateTime StartDate,
    DateTime EndDate,
    string TimeZone,
    string? Title,
    List<string>? Statuses,
    int? DurationMin,
    int? DurationMax,
    List<long>? ServiceId)
{
    public long UserId { get; set; }

    public GetMeetingRequest() : this(
        DateTime.Now.AddMonths(-1),
        DateTime.Now,
        null!,
        null,
        new List<string>(),
        null,
        null,
        new List<long>())
    { }
}
