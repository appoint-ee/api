using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class User
{
    [Key]
    public long Id { get; set; }

    [Required, StringLength(50)]
    public string UserName { get; set; }
    
    [Required, StringLength(50)]
    public string FirstName { get; set; }

    [Required, StringLength(50)]
    public string LastName { get; set; }

    [Required, StringLength(100)]
    [EmailAddress]
    public string EmailAddress { get; set; }

    public string? PhotoUrl { get; set; }

    [StringLength(20)]
    public string Status { get; set; }

    public string? Address { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    [StringLength(5)]
    public string? LangCode { get; set; }

    [StringLength(50)]
    public string? PreferredTimeZone { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<UserService> UserServices { get; set; }
}
