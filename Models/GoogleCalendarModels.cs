namespace api.Models;

public class GoogleCalendarEvents
{
    public string TimeZone { get; set; }
    public List<GoogleCalendarEventItem> Items { get; set; }
}

public class GoogleCalendarEventItem
{
    public string Id { get; set; } // the length of the Id must be between 5 and 1024 characters
    public string Status { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public GoogleCalendarEventDateTime Start { get; set; }
    public GoogleCalendarEventDateTime End { get; set; }
    public List<string>? Recurrence { get; set; }
}

public class GoogleCalendarEventDateTime
{
    public string Date { get; set; }
    public DateTime? DateTime { get; set; }
}