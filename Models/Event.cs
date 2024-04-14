using System.Text.Json.Serialization;

namespace api.Models;

public class Event
{
    public Event()
    {
        this.Start = new EventDateTime()
        {
            TimeZone = "Europe/Zurich"
        };
        this.End = new EventDateTime()
        {
            TimeZone = "Europe/Zurich"
        };
    }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("summary")]
    public string Summary { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; set; }
    
    [JsonPropertyName("start")]
    public EventDateTime Start { get; set; }
    
    [JsonPropertyName("end")]
    public EventDateTime End { get; set; }
    
    public class EventDateTime
    {
        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }
        
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }
    }
}