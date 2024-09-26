using api.Controllers.Models;

namespace api.Services;

public interface IGoogleAuthService
{
    void Store(StoreGoogleAuthRequest request);

    Task<string> Update(long userId, string refreshToken);
    
    GetGoogleAuthResponse? Get(long userId);
}