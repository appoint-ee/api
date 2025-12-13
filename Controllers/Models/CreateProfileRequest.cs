namespace api.Controllers.Models;

public class CreateProfileRequest
{
    public string ProfileName { get; set; }
    
    public string EmailAddress { get; set; }
    
    public string? PhotoUrl { get; set; }
    
    public string? Address { get; set; }

    public string? City { get; set; }
    
    public string? Country { get; set; }

    public string? LangCode { get; set; }

    public string? PreferredTimeZone { get; set; }
    
    public bool IsOrg { get; set; }
    
    public bool IsOnline { get; set; }

    public ICollection<long> UserIds { get; set; } = new List<long>();
}