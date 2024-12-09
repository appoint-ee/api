namespace api.Controllers.Models;

public class UpsertDateSpecificHourRequest
{
    public Guid? Id { get; init; }
    public DateOnly SpecificDate { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
}