namespace api.Services.Dtos;

public class GetUserResponse
{
    public long Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string EmailAddress { get; init; }
    public string? PhotoUrl { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string PhoneNumber { get; init; }
}