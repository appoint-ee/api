using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class Meeting
{
    [Key]
    public Guid Id { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; }

    public string Description { get; set; }

    [StringLength(1024)]
    public string? ExternalId { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<MeetingAttendee> Attendees { get; set; }
}