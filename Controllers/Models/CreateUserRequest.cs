namespace api.Controllers.Models;

// TODO: Decouple service and endpoint request objects
public class CreateUserRequest
{    
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string? PhotoUrl { get; init; }
}