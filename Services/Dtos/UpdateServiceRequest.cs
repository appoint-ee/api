namespace api.Services.Dtos;

public class UpdateServiceRequest
{
    public string Name { get; init; }
    public TimeSpan DefaultDuration { get; init; }
    public decimal DefaultPrice { get; init; }
}