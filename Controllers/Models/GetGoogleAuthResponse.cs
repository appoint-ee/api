namespace api.Controllers.Models;

public class GetGoogleAuthResponse
{
    public long UserId { get; init; }
    
    public string AccessToken { get; init; }

    public int ExpiresIn { get; init; }
    
    public string? RefreshToken { get; init; }
    
    public string Scope { get; init; }
    
    public string TokenType { get; init; }
    
    public string IdToken { get; init; }
}