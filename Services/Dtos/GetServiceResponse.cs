namespace api.Services.Dtos;

public class GetServiceResponse
{
    public long Id { get; init; }
    public string Name { get; init; }
    public TimeSpan DefaultDuration { get; init; }
    public decimal DefaultPrice { get; init; }
}