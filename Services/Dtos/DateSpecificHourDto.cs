namespace api.Services.Dtos;

public class DateSpecificHourDto
{
    public Guid Id { get; init; }
 
    public long UserId { get; init; }
    
    public DateOnly SpecificDate { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}