using System.ComponentModel.DataAnnotations;

namespace api.Controllers.Models;

public class CreateMeetingRequest
{
    [Required, StringLength(100)]
    public string Title { get; init; } // TODO: still needed?

    [Required, StringLength(100)]
    public string Description { get; init; } // TODO: still needed?
    
    public string CreatorFirstName { get; init; }
    
    public string CreatorLastName { get; init; }
    
    public string CreatorEmailAddress { get; init; }
    
    public long? CreatorId { get; init; }
    
    public long? HostId { get; init; }
    
    public string? ExternalId { get; init; }

    [Required]
    public DateTime StartTime { get; init; }

    [Required]
    public DateTime EndTime { get; init; }
    
    public long ServiceId { get; init; }
}