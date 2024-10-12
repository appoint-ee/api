using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class UserService
{
    public long UserId { get; set; }
    public User User { get; set; }
    
    public long ServiceId { get; set; }
    public Service Service { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
}