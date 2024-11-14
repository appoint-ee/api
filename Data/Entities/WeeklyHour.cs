using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class WeeklyHour
{
    [Key]
    public Guid Id { get; set; }
 
    [Required]
    public long UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    [Range(0, 6)]
    public int DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
}