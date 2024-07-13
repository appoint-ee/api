using System.ComponentModel.DataAnnotations;

namespace api.Controllers.Models;

public class CreateMeetingRequest
{
    [Required, StringLength(100)]
    public string Title { get; init; }

    [Required, StringLength(100)]
    public string Description { get; init; }
    
    public string CreatorFirstName { get; init; }
    
    public string CreatorLastName { get; init; }
    
    public string CreatorEmailAddress { get; init; }
    
    public long? CreatorId { get; init; }
    
    public long? ParticipantId { get; init; }
    
    public string ExternalId { get; init; }

    [Required]
    public DateTime StartTime { get; init; }

    [Required]
    public DateTime EndTime { get; init; }
}