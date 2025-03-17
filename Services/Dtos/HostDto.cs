namespace api.Services.Dtos;

public class HostDto
{
    public long Id { get; init; }
    
    public string FirstName { get; init; }

    public string LastName { get; init; }
    
    public string PhotoUrl { get; init; }
    
    public string Status { get; init; }

    public List<ServiceDto> Services { get; init; }
}