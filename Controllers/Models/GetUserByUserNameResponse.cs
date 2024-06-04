namespace api.Controllers.Models;

public class GetUserByUserNameResponse
{
    public long Id { get; init; }

    public string UserName { get; init; }
    
    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string EmailAddress { get; init; }

    public string PhotoUrl { get; init; }

    public string Status { get; init; }

    public string Address { get; init; }

    public string CountryCode { get; init; }

    public string LangCode { get; init; }

    public string PreferredTimeZone { get; init; }
}
