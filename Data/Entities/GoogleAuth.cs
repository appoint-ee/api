using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Data.Entities;

public class GoogleAuth
{
    [Key]
    public Guid Id { get; init; }
    
    [Required]
    public long UserId { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; }

    [Required]
    public string AccessToken { get; set; }
    
    [Required]
    public int ExpiresIn { get; set; }
    
    [Required]
    public string RefreshToken { get; set; }
    
    [Required]
    public string Scope { get; set; }
    
    [Required, StringLength(32)]
    public string TokenType { get; set; }
    
    [Required]
    public string IdToken { get; set; }
}