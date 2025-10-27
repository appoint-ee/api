namespace api.Services.Dtos;

public class ServiceDto
{
    public long Id { get; init; }
    
    public string? Name { get; init; }
    
    public TimeSpan Duration { get; init; }
    
    public decimal Price { get; init; }
    
    public List<HostDto> Hosts { get; init; }
}