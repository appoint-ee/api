using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.Data.Enums;

namespace api.Data.Entities;

public class MeetingAttendee
{
    [Key]
    public Guid Id { get; set; }

    public long? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    public bool IsHost { get; set; }

    public Guid MeetingId { get; set; }
    
    [ForeignKey("MeetingId")]
    public Meeting Meeting { get; set; }

    public AttendeeStatus Status { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}