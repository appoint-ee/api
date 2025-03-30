using System.ComponentModel.DataAnnotations;

namespace api.Controllers.Models;

public class CreateMeetingRequest
{
    public string? CreatorFirstName { get; init; }
    
    public string? CreatorLastName { get; init; }
    
    public string? CreatorEmailAddress { get; set; }
    
    public string? CreatorPhoneNumber { get; init; }
    
    public long? CreatorId { get; set; }
    
    public long? HostId { get; init; }
    
    [Required, StringLength(100)]
    public string Description { get; init; }
    
    [Required]
    public DateTime StartTime { get; init; }

    [Required]
    public DateTime EndTime { get; init; }
    
    public long ServiceId { get; init; }
}