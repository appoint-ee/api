using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class MeetingAttendee
{
    [Key]
    public Guid Id { get; set; }

    public long? UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    public bool IsCreator { get; set; }

    public Guid MeetingId { get; set; }
    
    [ForeignKey("MeetingId")]
    public Meeting Meeting { get; set; }

    [StringLength(20)]
    public string Status { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
}