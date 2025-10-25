using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class DateSpecificHour
{
    [Key]
    public Guid Id { get; set; }
 
    [Required]
    public long UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User? User { get; set; }
    
    public DateOnly SpecificDate { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}