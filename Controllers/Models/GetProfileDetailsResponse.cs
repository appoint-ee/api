namespace api.Controllers.Models;

public class GetProfileDetailsResponse
{
    public long Id { get; init; }

    public string UserName { get; init; }

    public string EmailAddress { get; init; }

    public string PhotoUrl { get; init; }

    public string Address { get; init; }

    public string CountryCode { get; init; }

    public string LangCode { get; init; }

    public string PreferredTimeZone { get; init; }
    
    public bool IsOrg { get; init; }
    
    public List<ServiceDto> Services { get; init; }
}

public class ServiceDto
{
    public long Id { get; init; }
    
    public string? Name { get; init; }
    
    public TimeSpan DefaultDuration { get; init; }
    
    public decimal DefaultPrice { get; init; }
    
    public List<HostDto> Hosts { get; init; } 
}

public class HostDto
{
    public long Id { get; init; }
    
    public string FirstName { get; init; }

    public string LastName { get; init; }
    
    public string PhotoUrl { get; init; }
    
    public TimeSpan Duration { get; init; }
    
    public decimal Price { get; init; }
}