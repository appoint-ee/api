using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class Meeting // TODO: consider renaming as appointment
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
    
    public long ServiceId { get; set; }
    
    [ForeignKey("ServiceId")]
    public Service Service { get; set; }
    
    [Required]
    public TimeSpan Duration { get; set; }

    [Required]
    [Range(0, 9999.99)]
    public decimal Price { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<MeetingAttendee> Attendees { get; set; }
}