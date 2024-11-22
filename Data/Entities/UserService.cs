using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class UserService
{
    public long UserId { get; set; }
    public User User { get; set; }
    
    public long ServiceId { get; set; }
    public Service Service { get; set; }
    
    [Required]
    public TimeSpan Duration { get; set; }
    
    [Required]
    [Range(0, 9999.99)]
    public decimal Price { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
}