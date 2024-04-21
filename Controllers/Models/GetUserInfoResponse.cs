namespace api.Controllers.Models;

public class GetUserInfoResponse
{
    public string Locale { get; init; }
    public string Name { get; init; }
    public string PhotoUrl { get; init; }
    public string Address { get; init; }
    public string EmailAddress { get; init; }
    public string PhoneNumber { get; init; }
}