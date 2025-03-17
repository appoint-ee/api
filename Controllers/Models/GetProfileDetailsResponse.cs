using api.Services.Dtos;

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
    
    public List<HostDto> Hosts { get; init; }
    
    public List<ServiceDto> Services { get; init; }
    
    public List<WeeklyHourDto> WeeklyHours { get; set; }
    
    public List<DateSpecificHourDto> DateSpecificHours { get; set; }
}