using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class Profile
{
    [Key]
    public long Id { get; set; }
    
    [Required, StringLength(50)]
    public string ProfileName { get; set; }
    
    [Required, StringLength(100)]
    [EmailAddress]
    public string EmailAddress { get; set; }
    
    public string? PhotoUrl { get; set; }
    
    public string? Address { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(5)]
    public string? LangCode { get; set; }

    [StringLength(50)]
    public string? PreferredTimeZone { get; set; }
    
    [Required]
    public bool IsOrg { get; set; }
    
    public ICollection<User> Users { get; set; } = new List<User>();
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
}