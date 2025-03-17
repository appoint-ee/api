namespace api.Services.Dtos;

public class UpdateUserRequest
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string EmailAddress { get; init; }
    public string? PhotoUrl { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string PhoneNumber { get; init; }
    public long? ProfileId { get; init; }
    public IEnumerable<ServiceDto> Services { get; init; }
}