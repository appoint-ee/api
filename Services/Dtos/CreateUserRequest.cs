namespace api.Services.Dtos;

// TODO: Decouple service and endpoint request objects
public class CreateUserRequest
{    
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string? PhotoUrl { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string PhoneNumber { get; init; }
}