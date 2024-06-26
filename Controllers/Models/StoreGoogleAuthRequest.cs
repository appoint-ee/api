using System.Text.Json.Serialization;

namespace api.Controllers.Models;

public class StoreGoogleAuthRequest
{
    [JsonPropertyName("user_id")]
    public long UserId { get; init; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; init; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; }
    
    [JsonPropertyName("id_token")]
    public string IdToken { get; init; }
}